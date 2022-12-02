package mrl.motion.critical.run;

import mrl.motion.data.Motion;
import mrl.motion.data.trasf.Pose2d;
import mrl.motion.neural.agility.AgilityControlParameterGenerator;
import mrl.motion.neural.agility.JumpSpeedModel;
import mrl.motion.neural.data.MotionDataConverter;
import mrl.motion.neural.run.RealtimePythonController;
import mrl.util.Configuration;
import mrl.util.MathUtil;
import mrl.util.Utils;
import mrl.widget.app.Item;

import javax.vecmath.Vector2d;
import javax.vecmath.Vector3d;
import java.io.IOException;
import java.util.LinkedList;

public class ModelRunner {
    private RealtimePythonController c;
    private LinkedList<Integer> actionQueue = null;
    private LinkedList<Integer> actionStartFrames = null;

    private Vector2d targetDirection;

    private double totalAgility = 1;
    private boolean useDynamicAgility = true;

    private int locoType = 1;

    private boolean isActivated = false;
    private int activatedCount = 0;


    public void init(String folder, int actionTypesLength) {
        MotionDataConverter.useTPoseForMatrix = false;
        MotionDataConverter.useMatrixForAll = true;

        c = new RealtimePythonController() {
            @Override
            public double[] getControlParameter() {
                if (targetDirection == null) return null;

                int locoSize = JumpSpeedModel.LOCO_ACTION_SIZE;
                Item.ItemDescription desc = new Item.ItemDescription(new Vector3d(1, 1, 0));
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
                double[] control = AgilityControlParameterGenerator.getActionType(actionTypesLength, action);
                control = MathUtil.concatenate(control, new double[] { targetAngle });
                if (useDynamicAgility) {
                    control = MathUtil.concatenate(control, new double[] { totalAgility });
                }
                return control;
            }
        };

        MotionDataConverter.setAllJoints();
        Configuration.BASE_MOTION_FILE = "data\\t_pose_ue2.bvh";
        MotionDataConverter.setUseOrientation();
        MotionDataConverter.setOrientationJointsByFileOrder();

        useDynamicAgility = true;
        c.init(folder);

        actionQueue = new LinkedList<Integer>();
        actionStartFrames = new LinkedList<Integer>();
        targetDirection = new Vector2d(1, 0);
        locoType = 0;
    }

    public Motion motion() {
        return c.g.motion();
    }

    public void reset() {
        c.reset();
        c.g.update(c.iterateMotion());
    }

    public double[] iterate() {
        double[] output = c.iterateMotion();
        c.g.update(output);
        Motion m = c.g.motion();
        return output;
    }

    public void doAction(int action) {
        actionQueue.push(action);
        actionStartFrames.push(c.frame);
    }

    public void setDirection(Vector2d vec) throws IOException {
        targetDirection = vec;
    }
    
    public void setTotalAgility(double val)
    {
    	totalAgility = val;
    }
}
