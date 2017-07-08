using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthBoss : PolygonGameObject 
{
	MEarthBossData data;
	AdvancedTurnComponent turnComponent;
	SpwnedArmData leftArm;
	SpwnedArmData rightArm;

	public void Init(MEarthBossData data){
		this.data = data;
		turnComponent = new AdvancedTurnComponent (this, data.rotationSpeed);
	}

	public override void BeforeAddToMainList () {
		base.BeforeAddToMainList ();
		leftArm = CreateArm (data.leftArm);
		rightArm = CreateArm (data.rightArm);
	}

	SpwnedArmData CreateArm(MEarthBossData.ArmData armData){
		var shoulder = CreatePart (armData.shoulder, this);
		var joint = CreatePart (armData.joint, shoulder.obj);
		var hand = CreatePart (armData.hand, joint.obj);
		return new SpwnedArmData{ shoulder = shoulder, joint = joint, hand = hand };
	}

	SpawnedJointData CreatePart(MEarthBossData.JointData jointData, PolygonGameObject relativeTo){
		var obj = jointData.spawn.Create ();
		obj.position = relativeTo.position;
		var angleEff = obj.AddEffect(new KeepOrientationEffect(jointData.orientationData, relativeTo));
		var posEff = obj.AddEffect(new KeepPositionEffect(jointData.positionData, relativeTo));
		Singleton<Main>.inst.Add2Objects (obj);
		return new SpawnedJointData{ obj = obj, angleEff = angleEff, posEff = posEff };
	}

	protected override void ApplyRotation (float dtime) {
		if (TargetNotNull) {
			turnComponent.TurnByDirection (target.position - this.position, dtime);
		} else {
			turnComponent.TurnByDirection (cacheTransform.right, dtime);
		}
	}

	public override void Tick (float delta)	{
		base.Tick (delta);

		ControlJointAngle (leftArm.shoulder, data.leftArmState, data.armNormalContractAngle, data.armMaxContractAngle, delta);
		ControlJointAngle (leftArm.joint, data.leftArmState, data.jointNormalContractAngle, data.jointMaxContractAngle, delta);

		ControlJointAngle (rightArm.shoulder, data.rightArmState, -data.armNormalContractAngle, -data.armMaxContractAngle, delta);
		ControlJointAngle (rightArm.joint, data.rightArmState, -data.jointNormalContractAngle, -data.jointMaxContractAngle, delta);
	}

	void ControlJointAngle(SpawnedJointData joint, MEarthBossData.ArmState state, float normalAngle, float maxAngle, float delta) {
		var angleEff = joint.angleEff;
		float desiredOffsetAngle = angleEff.extraOffsetAngle;
		float deltaAngle = delta * angleEff.rotaitingSpeed;
		switch (state) {
		case MEarthBossData.ArmState.HOLD:
			break;
		case MEarthBossData.ArmState.CONTRACT:
			desiredOffsetAngle = maxAngle;
			break;
		case MEarthBossData.ArmState.EXTEND:
			desiredOffsetAngle = 0;
			break;
		case MEarthBossData.ArmState.NORMAL:
			desiredOffsetAngle = normalAngle;
			break;
		default:
			break;
		}
		if (angleEff.extraOffsetAngle != desiredOffsetAngle) {
			angleEff.extraOffsetAngle = MoveTo (angleEff.extraOffsetAngle, deltaAngle, desiredOffsetAngle);
		}
	}

	float MoveTo(float value, float delta, float target){
		if (target > value) {
			return Mathf.Max (target, value + delta);
		} else {
			return Mathf.Min (target, value - delta);
		}
	}

	public class SpwnedArmData {
		public SpawnedJointData shoulder;
		public SpawnedJointData joint;
		public SpawnedJointData hand;
	}

	public class SpawnedJointData{
		public PolygonGameObject obj;
		public KeepOrientationEffect angleEff;
		public KeepPositionEffect posEff;
	}
}