package mrl.motion.critical.run;

import java.io.*;
import java.net.*;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.HashMap;
import java.util.LinkedList;

import javax.vecmath.Matrix4d;
import javax.vecmath.Point3d;
import javax.vecmath.Vector2d;
import javax.vecmath.Vector3d;

import mrl.motion.data.Motion;
//import org.eclipse.swt.events.KeyEvent;

import mrl.motion.data.SkeletonData;
import mrl.motion.data.trasf.Pose2d;
import mrl.motion.neural.agility.AgilityControlParameterGenerator;
import mrl.motion.neural.agility.JumpSpeedModel;
import mrl.motion.neural.data.MotionDataConverter;
import mrl.motion.neural.run.RealtimePythonController;
import mrl.util.Configuration;
import mrl.util.MathUtil;
import mrl.util.Utils;
import mrl.widget.app.Item.ItemDescription;

public class UnityConnectedMartialArts {
    private RealtimePythonController c;
    private LinkedList<Integer> actionQueue = null;
    private LinkedList<Integer> actionStartFrames = null;

    private Vector2d targetDirection;

    private double agility = 1;
    private boolean useDynamicAgility = true;

    private int locoType = 1;

    private boolean isActivated = false;
    private int activatedCount = 0;
    private ServerSocket server;
    private DataOutputStream socketWriter;
    private DataInputStream socketReader;
    private ByteArrayOutputStream baos;

    private ExtendedByteArrayOutputStream baosWriter;
    private String[] posJoints;
    private String[] rotJoints;
    private String[] matJoints;


    private void start() throws IOException {
        MotionDataConverter.useTPoseForMatrix = false;
        MotionDataConverter.useMatrixForAll = true;

        createRuntimeController();
        server = new ServerSocket(1369);

        while (true) {
            try {
                serveClient();
            } catch (Exception e) {
                e.printStackTrace();
                System.out.println(e);
            }
        }
    }

    private void sendInitialPayload() throws IOException {
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

        SkeletonData.Joint j = c.g.motion().motionData.skeletonData.root;
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

    private void sendOutputPayload(double[] output, Motion m) throws IOException {
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

    public interface MessageHandlerInterface {
       void handle(int payloadLength) throws IOException;
    }

    private int waitingBytes = -1;
    private final MessageHandlerInterface[] messageHandlers = {
            this::handlerDoAction,
            this::handlerSetDirection
    };

    private void receiveData() throws IOException {
        while (true) {
            int availBytes = socketReader.available();
            if (waitingBytes < 0 && availBytes >= 4) {
                waitingBytes = socketReader.readInt();
                continue;
            }

            if (waitingBytes > 0 && availBytes >= waitingBytes) {
                int opCode = socketReader.readInt();
                if (opCode < 0 || opCode >= messageHandlers.length){
                    socketReader.skipBytes(waitingBytes - 4);
                    System.out.println("Got unknown opcode " + opCode + " with payload length of " + (waitingBytes-4));
                }
                else {
                    System.out.println("OP" + opCode + ", Payload: " + (waitingBytes-4));
                    messageHandlers[opCode].handle(availBytes - 4);
                }
                waitingBytes = -1;
                continue;
            }

            break;
        }
    }

    private void handlerDoAction(int length) throws IOException {
        int action = socketReader.readInt();
        actionQueue.push(action);
        actionStartFrames.push(c.frame);
    }

    private void handlerSetDirection(int length) throws IOException {
        targetDirection = new Vector2d(socketReader.readFloat(), socketReader.readFloat());
    }



    private void serveClient() throws IOException, InterruptedException {
        System.out.println("waiting for client");
        Socket soc = server.accept();
        System.out.println("accepted " + soc.getRemoteSocketAddress());

        socketWriter = new DataOutputStream(soc.getOutputStream());
        socketReader = new DataInputStream(soc.getInputStream());

        baos = new ByteArrayOutputStream();
        baosWriter = new ExtendedByteArrayOutputStream(baos);

        c.reset();

        c.g.update(c.iterateMotion());

        posJoints = MotionDataConverter.KeyJointList_Origin;
        rotJoints = MotionDataConverter.OrientationJointList;
        matJoints = MotionDataConverter.OrientationJointList;

        sendInitialPayload();

        while (true) {
            receiveData();
            double[] output = c.iterateMotion();
            c.g.update(output);
            Motion m = c.g.motion();
            sendOutputPayload(output, m);

            baosWriter.flush();
            byte[] result = baos.toByteArray();
            baos.reset();
            socketWriter.writeInt(result.length);
            socketWriter.write(result);
            socketWriter.flush();
            Thread.sleep(33);
        }
    }

    private void createRuntimeController() {
        c = new RealtimePythonController() {
            @Override
            public double[] getControlParameter() {
                if (targetDirection == null) return null;

                int locoSize = JumpSpeedModel.LOCO_ACTION_SIZE;
                ItemDescription desc = new ItemDescription(new Vector3d(1, 1, 0));
                desc.size = 10;
                Pose2d goalPose = new Pose2d(g.pose.position, targetDirection);
                double targetAngle = MathUtil.directionalAngle(g.pose.direction, targetDirection);
                int action = locoType;

                if (actionQueue.size() > 0) {
                    action = actionQueue.getFirst();
                    int timePassed = frame - actionStartFrames.getFirst();
                    if (isActivated) {
                        activatedCount++;
                        if (activatedCount > 3) {
                            isActivated = false;
                            activatedCount = -1;

                            int prevAction = actionQueue.removeFirst();
                            actionStartFrames.removeFirst();
                            System.out.println("action finish : " + actionQueue.size() + " : " +  prevAction + " : " + frame + " : " + timePassed + " : " + Utils.last(prevOutput));

                            if (actionQueue.size() > 0) {
                                actionStartFrames.removeFirst();
                                actionStartFrames.addFirst(frame);
                            }
                        }
                    } else {
                        boolean isFinished = Utils.last(prevOutput) > 0.4;
                        if (timePassed > 5 && isFinished) {
                            isActivated = true;
                            activatedCount = 0;
                        }
                    }
                }
                if (action >= locoSize) {
                    targetAngle = Double.NaN;
                }
                double[] control = AgilityControlParameterGenerator.getActionType(MartialArtsConfig.actionTypes.length, action);
                control = MathUtil.concatenate(control, new double[] { targetAngle });
                if (useDynamicAgility) {
                    control = MathUtil.concatenate(control, new double[] { agility });
                }
                return control;
            }
        };

        MotionDataConverter.setAllJoints();
        Configuration.BASE_MOTION_FILE = "data\\t_pose_ue2.bvh";
        MotionDataConverter.setUseOrientation();
        MotionDataConverter.setOrientationJointsByFileOrder();

        useDynamicAgility = true;
        c.init("martial_arts_sp_da");

        actionQueue = new LinkedList<Integer>();
        actionStartFrames = new LinkedList<Integer>();
        targetDirection = new Vector2d(1, 0);
        locoType = 0;

    }

    public static void main(String[] args) {
        final UnityConnectedMartialArts instance = new UnityConnectedMartialArts();
        try {
            instance.start();
        } catch (Exception e) {
            e.printStackTrace();
            System.out.println(e);
        }
    }
}
