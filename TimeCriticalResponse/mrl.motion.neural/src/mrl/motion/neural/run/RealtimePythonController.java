package mrl.motion.neural.run;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.HashMap;

import javax.vecmath.Point3d;
import javax.vecmath.Tuple2d;

import mrl.util.Pair;
import org.eclipse.swt.SWT;

import mrl.motion.data.Motion;
import mrl.motion.data.MotionData;
import mrl.motion.data.trasf.MotionDistByPoints;
import mrl.motion.data.trasf.Pose2d;
import mrl.motion.neural.data.BallTrajectoryGenerator;
import mrl.motion.neural.data.MotionDataConverter;
import mrl.motion.neural.data.Normalizer;
import mrl.motion.position.PositionResultMotion;
import mrl.motion.viewer.module.MainViewerModule;
import mrl.util.Utils;
import mrl.widget.app.ItemListModule;
import mrl.widget.app.MainApplication;

public abstract class RealtimePythonController extends RuntimeController{
    public static boolean USE_NORMALIZATION = true;

    public MainApplication app;
    public RNNPython python;
    public PositionResultMotion totalMotion = new PositionResultMotion();
    public ArrayList<Motion> totalMotion2 = new ArrayList<Motion>();
    public ArrayList<double[]> inputList = new ArrayList<double[]>();

    public double[] prevOutput;

    public RealtimePythonController() {
        this(false);
    }
    public RealtimePythonController(boolean useBall) {
        MotionDataConverter.setAllJoints();
        if (!useBall) {
            MotionDataConverter.setNoBall();
        } else {
            RuntimeMotionGenerator.ALWAYS_HAS_BALL = true;
        }
    }

    public void init(String folder) {
        python = new RNNPython(folder, false);
        normal = new Normalizer(folder);
        g = new RuntimeMotionGenerator();

        double[] initialY = new double[normal.yMeanAndStd[0].length];
        python.model.setStartMotion(initialY);
        prevOutput = initialY;
    }

    public void reset() {
        if (normal.yList == null) return;
        double[] initialY = normal.yList.get(3);
        initialY = new double[initialY.length];
        python.model.setStartMotion(initialY);
        prevOutput = initialY;
        g.pose = new Pose2d(Pose2d.BASE);
    }

    public abstract double[] getControlParameter();

    public double[] iterateMotion(){
        double[] x = getControlParameter();
        frame++;
        if (x == null) return null;
        inputList.add(x);
        if (USE_NORMALIZATION) x = normal.normalizeX(x);
        double[] output = predict(x);
        prevOutput = output;
        if (USE_NORMALIZATION) output = normal.deNormalizeY(output);
        return output;
    }

    protected double[] predict(double[] x) {
        return python.model.predict(x);
    }
}
