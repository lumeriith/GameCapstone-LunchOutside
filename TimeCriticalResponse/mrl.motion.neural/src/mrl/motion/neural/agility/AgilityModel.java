package mrl.motion.neural.agility;

import java.util.ArrayList;

import mrl.motion.data.MDatabase;
import mrl.motion.data.Motion;
import mrl.motion.data.trasf.Pose2d;
import mrl.motion.dp.TransitionActionDP;
import mrl.motion.dp.TransitionData;
import mrl.motion.neural.agility.match.MMatching;
import mrl.motion.neural.agility.match.MotionMatching;
import mrl.motion.neural.agility.match.TransitionDPGenerator;
import mrl.util.MathUtil;
import mrl.util.Utils;

public abstract class AgilityModel {
	
	public static int GOAL_TIME_LIMIT = 20;
	public static int TIME_EXTENSION_MIN_TIME = 10;
	public static double TIME_EXTENSION_RATIO = 0.33;
	public static int ACTIVATION_RISE = 3;
	public static int ACTIVATION_PEAK = 1;
	
	public AgilityModel() {
	}
	
	public String[] getActionTypes() {
		throw new RuntimeException();
	}
	public String[] getFullActionTypes() {
		return getActionTypes();
	}
	
	public int getContinuousLabelSize() {
		throw new RuntimeException();
	}
	
	public int getActionSize() {
		return getActionTypes().length;
	}
	public abstract AgilityGoal sampleRandomGoal(Pose2d currentPose);
	public abstract AgilityGoal sampleIdleGoal();
	public abstract MotionMatching makeMotionMatching(MDatabase database);
	
	public MMatching makeMMatching(MDatabase database) {
		TransitionData tData = new TransitionData(database, getFullActionTypes(), getContinuousLabelSize());
		TransitionActionDP dp = new TransitionActionDP(tData);
		dp.load(TransitionDPGenerator.FILE_PREFIX + database.getDatabaseName() + ".dat");
		MMatching matching = new MMatching(dp);
		return matching;
	}
	
	public boolean useActivation() {
		return false;
	}
	
	protected double[] actionData(int actionType) {
		return AgilityControlParameterGenerator.getActionType(getActionSize(), actionType);
	}
	
	// search model?
//	public abstract double[] inferenceControlParameter();
	
	public abstract class AgilityGoal{
		public int actionType;
		public int timeLimit;
		public int maxSearchTime;
		public int minSearchTime;
		public double agility;
		
		public AgilityGoal(int actionType, int timeLimit) {
			this.actionType = actionType;
			setTime(timeLimit);
			
			agility = GOAL_TIME_LIMIT;
		}
		
		public Boolean checkActionValid(Motion currentMotion, int currentMotionAction, Motion targetMotion, int targetMotionAction) {
			return null;
		}
			
		/**
		 * ���� path���� motion�� ���� ���¿��� transition �� future pose �� rotation�� ��ġ�Ѵٰ� �������� error
		 * @param currentMoved
		 * @param futurePose
		 * @param futureRotation
		 * @return
		 */
		public double getSpatialError(Pose2d currentMoved, double currentRotated, Pose2d futurePose, double futureRotation) {
			return 0;
		}
		
		/**
		 * ���������� ������ motion�� editing �ϱ� ���� constraint pose
		 * @return
		 */
		public abstract Pose2d getEditingConstraint();
		
		public Pose2d getEditingConstraint(Motion first, Motion last) {
			throw new RuntimeException();
		}
		
		/**
		 * �н��� network�� Agility�� �� �Ҷ�, ������� ������ motion�� goal�� �޼��ߴ��� �Ǵ��ϴ� �Լ�
		 * @param startPose
		 * @param motionList
		 * @param activationList TODO
		 * @return
		 */
		public abstract boolean isGoalFinished(Pose2d startPose, ArrayList<Motion> motionList, ArrayList<Double> activationList);
		/**
		 * �н��� network�� Agility�� �� �Ҷ�, ���� motion�� �����ϱ� ���� controller�� �Ѱ��� ���ڸ� ������ִ� �Լ�
		 * @param poseList
		 * @return
		 */
		public abstract double[] getControlParameter(ArrayList<Pose2d> poseList);
		
		/**
		 * �н� �����͸� �����Ҷ�, ������ motion���κ��� �׿� �´� control parameter�� �����ϴ� �Լ�.
		 * @param motionSequence
		 * @param currentIndex
		 * @param targetIndex
		 * @return
		 */
		public abstract double[] getControlParameter(ArrayList<Motion> motionSequence, int currentIndex, int targetIndex);
		
		/**
		 * �н��� network�� Agility�� �� �Ҷ�, ���� motion�� �����ϱ� ���� controller�� �Ѱ��� ���ڸ� �ð�ȭ �ϱ� ���� object�� ��ȯ�ϴ� �Լ�
		 * @param startPose
		 * @param currentPose
		 * @return
		 */
		public abstract Object getControlParameterObject(Pose2d startPose, Pose2d currentPose);
		
		
		/**
		 * Search �������� valid�� frame�� ���������� ���̻� Ž���� �����ϰ� �ش� path�� ���� path�� Ȯ������ ����
		 * @param goal
		 * @return
		 */
		public boolean isActiveAction() {
			return false;
		}
		
		public double getActivation(int remainTime) {
			if (!useActivation()) return Double.NaN;
			
			double activation = 0;
			if (isActiveAction()) {
				int activMargin = AgilityModel.ACTIVATION_PEAK + AgilityModel.ACTIVATION_RISE;
				if (remainTime <= activMargin) {
					if (remainTime <= AgilityModel.ACTIVATION_PEAK) {
						activation = 1;
					} else {
						activation = 1 - (remainTime - AgilityModel.ACTIVATION_PEAK)/(double)(AgilityModel.ACTIVATION_RISE + 1);
					}
				}
			}
			return activation;
		}
		
		public double[] getControlParameter() {
			return getControlParameter(Utils.singleList(Pose2d.BASE));
		}
		
		public void setTime(int timeLimit) {
			setTime(timeLimit, AgilityModel.TIME_EXTENSION_MIN_TIME, timeLimit + MathUtil.round(timeLimit*AgilityModel.TIME_EXTENSION_RATIO));
		}
				
		protected void setTime(int timeLimit, int minSearchTime, int maxSearchTime) {
			this.timeLimit = timeLimit;
			this.maxSearchTime = Math.min(maxSearchTime, 65);
			this.minSearchTime = minSearchTime;
		}
		
		public void increaseTime(int t) {
			timeLimit += t;
			maxSearchTime += t;
			minSearchTime += t;
		}
	}
}
