package mrl.motion.critical.run;

import mrl.motion.annotation.MotionAnnotationRun;

/**
 * @author user
 *
 */
public class MAnnotationRun {

	public static void main(String[] args) {
		//String dataFolder = "data\\martial_arts_compact";
		String dataFolder = "data\\duel";
		
		boolean openTransition = true;
//		openTransition = true;
		MotionAnnotationRun.open(dataFolder, !openTransition, true);
	}
}
