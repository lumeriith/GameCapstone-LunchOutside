package mrl.motion.critical.run;

import java.io.*;
import java.net.*;
import java.util.HashMap;
import java.util.LinkedList;

import javax.vecmath.Matrix4d;
import javax.vecmath.Point3d;
import javax.vecmath.Vector2d;
import javax.vecmath.Vector3d;

import mrl.motion.data.Motion;
import mrl.motion.data.MotionData;
import mrl.motion.data.SkeletonData;
import mrl.motion.position.PositionResultMotion;
//import org.eclipse.swt.events.KeyEvent;

import mrl.motion.data.trasf.Pose2d;
import mrl.motion.neural.agility.AgilityControlParameterGenerator;
import mrl.motion.neural.agility.JumpSpeedModel;
import mrl.motion.neural.data.MotionDataConverter;
import mrl.motion.neural.run.RealtimePythonController;
import mrl.util.Configuration;
import mrl.util.MathUtil;
import mrl.util.Pair;
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
    private void start() throws IOException {
        MotionDataConverter.useOrientation = true;
        MotionDataConverter.useTPoseForMatrix = false;
        MotionDataConverter.useMatrixForAll = false;

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

    private void serveClient() throws IOException, InterruptedException {
        System.out.println("waiting for client");
        Socket soc = server.accept();
        System.out.println("accepted " + soc.getRemoteSocketAddress());

        c.reset();

        DataOutputStream socketWriter = new DataOutputStream(soc.getOutputStream());

        ByteArrayOutputStream baos = new ByteArrayOutputStream();
        DataOutputStream baosWriter = new DataOutputStream(baos);

        String[] posJoints = MotionDataConverter.KeyJointList_Origin;
        String[] rotJoints = MotionDataConverter.OrientationJointList;

        baosWriter.writeInt(posJoints.length);
        baosWriter.writeInt(rotJoints.length);

        for (String posJoint : posJoints) {
            baosWriter.writeInt(posJoint.length());
            baosWriter.writeChars(posJoint);
        }
        for (String rotJoint : rotJoints) {
            baosWriter.writeInt(rotJoint.length());
            baosWriter.writeChars(rotJoint);
        }

        while (true) {
            double[] output = c.iterateMotion();
            HashMap<String, Point3d> posMap = MotionDataConverter.dataToPointMapByPosition(output);
            HashMap<String, Point3d> rotMap = MotionDataConverter.dataToPointMapByOrientation(output);

            for (String key : posJoints) {
                Point3d pos = posMap.get(key);
                baosWriter.writeFloat((float)pos.x);
                baosWriter.writeFloat((float)pos.y);
                baosWriter.writeFloat((float)pos.z);
            }
            for (String key : rotJoints) {
                Point3d rot = rotMap.get(key);
                baosWriter.writeFloat((float)rot.x);
                baosWriter.writeFloat((float)rot.y);
                baosWriter.writeFloat((float)rot.z);
            }

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
                // System.out.println("activation : " + agility + " : " + actionQueue.size() + " : " + action + " : " + frame + " : " + Utils.toString(Utils.last(prevOutput)));
                // System.out.println(Arrays.toString(control));
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
