package mrl.motion.critical.run;

import java.io.*;
import java.net.*;
import java.util.ArrayList;
import java.util.HashMap;

import javax.vecmath.Matrix4d;
import javax.vecmath.Point3d;
import javax.vecmath.Vector2d;
import javax.vecmath.Vector3d;

import mrl.motion.data.Motion;
//import org.eclipse.swt.events.KeyEvent;

import mrl.motion.data.SkeletonData;
import mrl.motion.neural.data.MotionDataConverter;

public class ModelBackendServer {
    public ModelRunner model;
    public ServerSocket server;
    public DataOutputStream socketWriter;
    public DataInputStream socketReader;

    private ModelMessageHandler messageHandler;
    private ByteArrayOutputStream baos;

    private ExtendedByteArrayOutputStream baosWriter;
    private String[] posJoints;
    private String[] rotJoints;
    private String[] matJoints;
    public float timescale = 1f;


    private int waitingBytes = -1;

    private void start() throws IOException {
        model = new ModelRunner();
        model.init("walk_10000_sp_da", WalkConfig.actionTypes.length);

        messageHandler = new ModelMessageHandler(this);

        int port = 1369;
        while (true) {
            try {
                server = new ServerSocket(port);
                System.out.println("using port " + port);
                break;
            }
            catch (BindException e) {
                port++;
            }
        }

        while (true) {
            try {
                serveClient();
            } catch (SocketException e) {
                System.out.println(e);
            }
            catch (Exception e) {
                e.printStackTrace();
                System.out.println(e);
            }
        }
    }

    private void sendSetupData() throws IOException {
        baosWriter.writeInt(posJoints.length);
        for (String posJoint : posJoints) {
            baosWriter.writeString(posJoint);
        }

        baosWriter.writeInt(rotJoints.length);
        for (String rotJoint : rotJoints) {
            baosWriter.writeString(rotJoint);
        }

        baosWriter.writeInt(matJoints.length);
        for (String matJoint : matJoints) {
            baosWriter.writeString(matJoint);
        }

        SkeletonData.Joint j = model.motion().motionData.skeletonData.root;
        ArrayList<SkeletonData.Joint> unpacked = new ArrayList<>();
        ArrayList<SkeletonData.Joint> unchecked = new ArrayList<>();
        unchecked.add(j);
        while (unchecked.size() > 0){
            j = unchecked.get(0);
            unchecked.remove(j);

            unpacked.add(j);
            unchecked.addAll(j.children);
        }

        baosWriter.writeInt(unpacked.size());
        for (int i = 0; i < unpacked.size(); i++) {
            SkeletonData.Joint joint = unpacked.get(i);
            baosWriter.writeString(joint.name);
            baosWriter.writeVector3d(joint.transition);
            baosWriter.writeInt(unpacked.indexOf(joint.parent));
        }
    }

    private void sendData(double[] output, Motion m) throws IOException {
        HashMap<String, Point3d> posMap = MotionDataConverter.dataToPointMapByPosition(output);
        HashMap<String, Vector3d> rotMap = MotionDataConverter.dataToOrientation(output);

        for (String key : posJoints) {
            Point3d pos = posMap.get(key);
            baosWriter.writePoint3d(pos);
        }
        for (String key : rotJoints) {
            Vector3d rot = rotMap.get(key);
            baosWriter.writeVector3d(rot);
        }
        for (String key : matJoints) {
            Matrix4d mat = m.get(key);
            baosWriter.writeMatrix4d(mat);
        }
    }

    private void receiveData() throws IOException {
        while (true) {
            int availBytes = socketReader.available();
            if (waitingBytes < 0 && availBytes >= 4) {
                waitingBytes = socketReader.readInt();
                continue;
            }

            if (waitingBytes > 0 && availBytes >= waitingBytes) {
                int opCode = socketReader.readInt();
                messageHandler.handleMessage(opCode, availBytes - 4);
                waitingBytes = -1;
                continue;
            }
            break;
        }
    }

    private void serveClient() throws IOException, InterruptedException {
        System.out.println("waiting for client");
        Socket soc = server.accept();
        System.out.println("accepted " + soc.getRemoteSocketAddress());

        socketWriter = new DataOutputStream(soc.getOutputStream());
        socketReader = new DataInputStream(soc.getInputStream());

        baos = new ByteArrayOutputStream();
        baosWriter = new ExtendedByteArrayOutputStream(baos);

        model.reset();
        timescale = 1f;

        posJoints = MotionDataConverter.KeyJointList_Origin;
        rotJoints = MotionDataConverter.OrientationJointList;
        matJoints = MotionDataConverter.OrientationJointList;

        sendSetupData();

        while (true) {
            receiveData();
            double[] output = model.iterate();
            sendData(output, model.motion());

            baosWriter.flush();
            byte[] result = baos.toByteArray();
            baos.reset();
            socketWriter.writeInt(result.length);
            socketWriter.write(result);
            socketWriter.flush();
            Thread.sleep((int)(1000f / (60f * timescale)));
        }
    }

    public static void main(String[] args) {
        final ModelBackendServer instance = new ModelBackendServer();
        try {
            instance.start();
        } catch (Exception e) {
            e.printStackTrace();
            System.out.println(e);
        }
    }
}
