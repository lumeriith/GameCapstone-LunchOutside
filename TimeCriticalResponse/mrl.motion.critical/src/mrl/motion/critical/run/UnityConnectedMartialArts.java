package mrl.motion.critical.run;

import java.io.*;
import java.net.*;
import java.util.Arrays;
import java.util.LinkedList;

import javax.vecmath.Point3d;
import javax.vecmath.Vector2d;
import javax.vecmath.Vector3d;

import mrl.motion.position.PositionResultMotion;
//import org.eclipse.swt.events.KeyEvent;

import mrl.motion.data.trasf.Pose2d;
import mrl.motion.neural.agility.AgilityControlParameterGenerator;
import mrl.motion.neural.agility.JumpSpeedModel;
import mrl.motion.neural.data.MotionDataConverter;
import mrl.motion.neural.run.PythonRuntimeController;
import mrl.util.Configuration;
import mrl.util.MathUtil;
import mrl.util.Utils;
import mrl.widget.app.Item.ItemDescription;

public class UnityConnectedMartialArts {
    private PythonRuntimeController c;
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

        DataOutputStream socketWriter = new DataOutputStream(soc.getOutputStream());

        ByteArrayOutputStream baos = new ByteArrayOutputStream();
        DataOutputStream baosWriter = new DataOutputStream(baos);

        while (true) {
            PositionResultMotion.PositionFrame frame = c.iterateMotion().get(0);

            baosWriter.writeInt(frame.size());
            for (int i = 0; i < frame.size(); i++) {
                Point3d[] data = frame.get(i);
                baosWriter.writeFloat((float) data[0].x);
                baosWriter.writeFloat((float) data[0].y);
                baosWriter.writeFloat((float) data[0].z);
                baosWriter.writeFloat((float) data[1].x);
                baosWriter.writeFloat((float) data[1].y);
                baosWriter.writeFloat((float) data[1].z);
            }

            baosWriter.flush();
            byte[] result = baos.toByteArray();
            baos.reset();
            socketWriter.writeInt(result.length);
            socketWriter.write(result);
            socketWriter.flush();
            Thread.sleep(100);
        }
    }

    private void createRuntimeController() {
        c = new PythonRuntimeController() {
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
