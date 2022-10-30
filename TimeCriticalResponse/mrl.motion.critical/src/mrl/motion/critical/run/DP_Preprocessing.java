package mrl.motion.critical.run;

import mrl.motion.neural.agility.match.TransitionDPGenerator;

public class DP_Preprocessing {

	public static void main(String[] args) {
//		String[] actions = MartialArtsConfig.actionTypes;
//		int cLabelSize = MartialArtsConfig.LOCO_ACTION_SIZE;
//		
//		String dataFolder = "martial_arts_compact";
//		String tPoseFile = "data\\t_pose_ue2.bvh";
		// locomotion action size(cyclic actions)
//		TransitionDPGenerator.printDistribution = false;
		
		String[] actions = DuelConfig.actionTypes;
		int cLabelSize = DuelConfig.LOCO_ACTION_SIZE;
		
		String dataFolder = "duel";
		String tPoseFile = "data\\stop_fencing.bvh";
		
		TransitionDPGenerator.make(dataFolder, tPoseFile, actions, cLabelSize);
	}
}
