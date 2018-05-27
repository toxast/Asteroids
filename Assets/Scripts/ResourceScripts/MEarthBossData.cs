using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MEarthBossData : MPolygonData {

	public float rotationSpeed;
	public MPolygonData helmetPrefab;
	public Place helmetPlace;

	public ArmData leftArm;
	public ArmData rightArm;
	public float armMaxContractAngle = 30;
	public float armNormalContractAngle = 10;
	public float jointMaxContractAngle = 110f;
	public float jointNormalContractAngle = 90f;
	public float contractDuration = 4f;
	public float shootAngle = 5f;

	[Header ("override arms")]
	public bool overrideArmsState = false;
	public ArmState leftArmState =  ArmState.HOLD;
	public ArmState rightArmState =  ArmState.HOLD;

	public MGunSetupData gun;
	public MSingleSpawn shoot2Spawn;
	public bool chargeHitAttack = false;
	public float rotationMultiplier = 3f;

	public bool throwArmAttack = false;
	public bool shootAttck = false;

	[Header ("shoot2")]
	public bool shootAttack2 = false;
	[Space (5f)]
	public float shoot2duration = 20f;
	public float shoot2aimVelocity = 20f;
	public float shoot2delay = 5f;
	public float shoot2forceDuration = 6f;
	public Place shoot2spawnPlace;
	public float shoot2SpreadDeg = 15f;
	public float shoot2accelerateForce = 40f;
	public float shoot2maxVel = 20f;
	public float shoot2SaccStabl = 1f;


	public EffectBetweenTwoObjects effectArm2Head;
	public ParticleSystemsData jointEffect;
	public ParticleSystemsData handEffect;

	public List<ParticleSystemsData> minigunHandAttackEffect;


	[System.Serializable]
	public class EffectBetweenTwoObjects {
		public ParticleSystemsData effect;
		public Place secondObjectPlace;
	}

	public enum ArmState{
		HOLD,
		NORMAL,
		CONTRACT,
		EXTEND,
	}

	//[SerializeField] bool val = false;
	void OnValidate(){
		leftArm.shoulder.orientationData.rotaitingSpeed = armMaxContractAngle / contractDuration;
		leftArm.joint.orientationData.rotaitingSpeed = jointMaxContractAngle / contractDuration;

		rightArm.shoulder.orientationData.rotaitingSpeed = armMaxContractAngle / contractDuration;
		rightArm.joint.orientationData.rotaitingSpeed = jointMaxContractAngle / contractDuration;
	}


	protected override PolygonGameObject CreateInternal (int layerNum)	{
		var spawn = PolygonCreator.CreatePolygonGOByMassCenter<EarthBoss> (verts, color);
		spawn.InitPolygonGameObject (physical);
		spawn.SetLayerNum (layerNum);
		spawn.targetSystem = new TargetSystem (spawn);
		spawn.Init (this);
		return spawn;
	}

	[System.Serializable]
	public class JointData{
		public MPolygonData spawn;
		public KeepOrientationEffect.Data orientationData;
		public KeepPositionEffect.Data positionData;
	}

	[System.Serializable]
	public class ArmData{
		public JointData shoulder;
		public JointData joint;
		public JointData hand;
	}
}


