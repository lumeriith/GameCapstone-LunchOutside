package mrl.motion.critical.run;

import mrl.motion.neural.gmm.GMMConfig;
import mrl.motion.neural.rl.PolicyLearning;

public class LearningTeacherPolicy {

	public static GMMConfig martial_arts() {
		String name = "martial_arts";
		return new MartialArtsConfig(name).setDataFolder("martial_arts_compact", "data\\t_pose_ue2.bvh");
	}
	
	public static GMMConfig duel() {
		String name = "duel";
		return new DuelConfig(name).setDataFolder("duel", "data\\stop_fencing.bvh");
	}
	
	public static GMMConfig duel_0() {
		String name = "duel_0";
		return new DuelConfig(name).setDataFolder("duel_0", "data\\stop_fencing.bvh");
	}
	
	public static GMMConfig walk() {
		String name = "walk_no_dir";
		return new WalkConfig(name).setDataFolder("walk", "data\\stop_fencing.bvh");
	}
	
	public static void main(String[] args) {
		//GMMConfig config = martial_arts();
		//GMMConfig config = duel();
		//GMMConfig config = duel_0();
		GMMConfig config = walk();
		PolicyLearning learning = new PolicyLearning(config, false);
		learning.runTraining(10000);
	}
}
