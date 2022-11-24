package mrl.motion.critical.run;

import mrl.motion.annotation.MotionAnnotationRun;

/**
 * @author user
 *
 */
public class MAnnotationRun {

	public static void main(String[] args) {
		//String dataFolder = "data\\martial_arts_compact";
		//String dataFolder = "data\\duel";
		//String dataFolder = "data\\duel_0";
		//String dataFolder = "data\\walk";
		String dataFolder = "data\\fencing";
		
		boolean openTransition = false;
//		openTransition = true;
		MotionAnnotationRun.open(dataFolder, !openTransition, true);
	}
}
