package mrl.motion.critical.run;

import mrl.motion.data.Motion;
import mrl.motion.data.trasf.Pose2d;
import mrl.motion.neural.agility.RotationModel;
import mrl.motion.neural.gmm.GMMConfig;
import mrl.motion.neural.rl.PolicyEvaluation.MatchingPath;
import mrl.util.MathUtil;

public class DuelConfig extends GMMConfig{
	
	public static double LOCOMOTION_RATIO = 0.8;
	public static int LOCO_ACTION_SIZE = 2;
	public static String[] actionTypes = {
			"walk",
			"run",
//			"walk_f",
//			"walk_b",
//			"walk_l",
//			"walk_r",
//			"run_f",
//			"run_b",
//			"run_l",
//			"run_r",
			"attack_short",
			"attack_long",
			"parry_up",
			"parry_down",
			"parry_left",
			"parry_right",
	};
	public static String[] fullActionTypes = actionTypes;
	
	static int tBase = 10;
	public static double[][] timeOffset = {
			{ tBase, tBase }, // walk
			{ tBase, tBase }, // run
//			{ tBase, tBase }, // walk_f
//			{ tBase, tBase }, // walk_b
//			{ tBase, tBase }, // walk_l
//			{ tBase, tBase }, // walk_r
//			{ tBase, tBase }, // run_f
//			{ tBase, tBase }, // run_b
//			{ tBase, tBase }, // run_l
//			{ tBase, tBase }, // run_r
			{ tBase, tBase }, // attack_short
			{ tBase, tBase }, // attack_long
			{ tBase, tBase }, // parry_up
			{ tBase, tBase }, // parry_down
			{ tBase, tBase }, // parry_left
			{ tBase, tBase }, // parry_right
	};

	public DuelConfig(String name) {
		super(name, fullActionTypes, actionTypes, LOCO_ACTION_SIZE);
	}
	
	@Override
	public GMMGoalGenerator makeGoalGenerator() {
		return new DuelGoalGenerator();
	}
	
	public static boolean USE_STRAIGHT_SAMPLING = true;
	private class DuelGoalGenerator extends GMMGoalGenerator{
		
		
		private boolean sampleStraight = false;
		private GMMStuntLocoGoal lastGoal;
		private Pose2d prevPose = new Pose2d(Pose2d.BASE);
		private Motion prevMotion = null;

		private int locoCount = 0;
		private int locoType = 0;
		
		public DuelGoalGenerator() {
			prevPose = new Pose2d(Pose2d.BASE);
			lastGoal = new GMMStuntLocoGoal(MathUtil.random.nextInt(getActionSize()), tBase, 0);
		}

		private boolean isActive(int action) {
			return action >= LOCO_ACTION_SIZE;
		}
		
		@Override
		public GMMGoal sampleRandomGoal(Pose2d currentPose, Motion currentMotion) {
			int actionType;
			double rotation;
			double lRatio = LOCOMOTION_RATIO;
			
			boolean isLastActive = isActive(lastGoal.actionType);
			if (isLastActive) {
				lRatio = 0.3;
			}
			
			if (MathUtil.random.nextDouble() < lRatio) {
				actionType = locoType;
				rotation = RotationModel.sampleRotation();
				if (isLastActive) {
					rotation = MathUtil.directionalAngle(currentPose.direction, prevPose.direction);
					sampleStraight = false;
				} else if (USE_STRAIGHT_SAMPLING) {
					if (sampleStraight) {
						rotation /= 20;
					}
					sampleStraight = !sampleStraight;
					if (Math.abs(rotation) < Math.PI / 4) {
						sampleStraight = false;
					}
				}
			} else {
				actionType = MathUtil.random.nextInt(actionTypes.length - LOCO_ACTION_SIZE) + LOCO_ACTION_SIZE;
				if (CONTROL_ACTION_DIRECTION) {
					rotation = 0.33*RotationModel.sampleRotation();
					if (isLastActive && prevMotion != null) {
						rotation += lastGoal.getDirectionOffset(prevMotion);
					}
				} else {
					if (isLastActive) {
						currentPose = prevPose;
					}
					rotation = 0;
				}
			}
			double timeLength = 0;
			timeLength += 8; // base margin
			timeLength += timeOffset[lastGoal.actionType][1];
			timeLength += timeOffset[actionType][0];
			if (!isActive(actionType) && isActive(lastGoal.actionType)) {
				timeLength += 10;
			}
			int adjustedTime = MathUtil.round(timeLength*TIME_RATIO);
			
			if (!isActive(actionType)) {
				locoCount++;
				if (actionType == 0) {
					locoCount += 3;
					rotation /= 60;
				}
				if (locoCount > 10) {
					locoCount = 0;
					locoType = MathUtil.random.nextInt(LOCO_ACTION_SIZE);
				}
			}
			
			lastGoal = new GMMStuntLocoGoal(actionType, adjustedTime, rotation);
			
			prevPose = currentPose;
			prevMotion = currentMotion;
			return lastGoal;
		}
		
	}

	public class GMMStuntLocoGoal extends GMMGoal{

		public GMMStuntLocoGoal(int actionType, int timeLimit, double direction) {
			super(actionType, timeLimit, direction);
		}
		
		@Override
		public double evaluateFinalError(MatchingPath path, double futureRotation, Pose2d futurePose, Motion finalMotion) {
			double rot = path.rotation + futureRotation;
			if (CONTROL_ACTION_DIRECTION && isActiveAction()) {
				double d = MathUtil.trimAngle(direction - getDirectionOffset(path.current) - rot);
				return d*d;
			}
			if (!isDirectionControl()) {
				return 0;
			} else {
				double d = MathUtil.trimAngle(direction - rot);
				return d*d;
			}
		}
		
		public double getDirectionOffset(Motion m) {
			String type = actionTypes[actionType];
			return getDirectionOffsetByType(m, type);
		}
		
		@Override
		public boolean isDirectionControl() {
			if (CONTROL_ACTION_DIRECTION && isActiveAction()) return true;
			return !isActiveAction() && actionType != 0;
		}
		
	}
	
	public static double getDirectionOffsetByType(Motion m, String type) {
		if (type.equals("parry_up") || 
				type.equals("parry_down") ||
				type.equals("parry_left") ||
				type.equals("parry_right")){
			return m._directionOffset("LeftHand", "RightHand");
		} 
		else if (
				type.equals("attack_short") ||
				type.equals("attack_long")
				) {
			return m._directionOffset("LeftFoot", "RightFoot");
		}
//		else if(type.equals("walk_b") ||
//				type.equals("run_b")
//				) {
//			return -Math.PI;
//		}
//		
//		else if(type.equals("walk_l") ||
//				type.equals("run_l")
//				) {
//			return -(Math.PI / 2);
//		}
//		
//		else if(type.equals("walk_r") ||
//				type.equals("run_r")
//				) {
//			return (Math.PI / 2);
//		}
		
		else {
			return 0;
		}
	}
}