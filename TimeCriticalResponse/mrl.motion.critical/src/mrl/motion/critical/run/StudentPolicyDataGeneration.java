package mrl.motion.critical.run;

import mrl.motion.neural.data.MotionDataConverter;
import mrl.motion.neural.gmm.GMMConfig;
import mrl.motion.neural.rl.PolicyDataGeneration;

public class StudentPolicyDataGeneration {

	public static void main(String[] args) {
		//GMMConfig config = LearningTeacherPolicy.martial_arts();
		//GMMConfig config = LearningTeacherPolicy.duel_0();
		//GMMConfig config = LearningTeacherPolicy.walk();
		GMMConfig config = LearningTeacherPolicy.fencing();
		
		MotionDataConverter.setOrientationJointsByFileOrder();
		String namePostfix = "_sp_da";
		new PolicyDataGeneration(config).generateData(namePostfix, 300000);
	}
}
