using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MEarthBossData : MPolygonData {

	[SerializeField] public float rotationSpeed;

	[SerializeField] public MPolygonData shoulder;
	[SerializeField] public KeepOrientationEffect.Data shoulderOrientationData;
	[SerializeField] public KeepPositionEffect.Data shoulderPositionData;
	[SerializeField] public ArmData leftArm;
	[SerializeField] public ArmData rightArm;
	[SerializeField] public float armMaxContractAngle = 30;
	[SerializeField] public float armNormalContractAngle = 10;
	[SerializeField] public float jointMaxContractAngle = 110f;
	[SerializeField] public float jointNormalContractAngle = 90f;

	public enum ArmState{
		HOLD,
		NORMAL,
		CONTRACT,
		EXTEND,
	}

	[SerializeField] public ArmState leftArmState =  ArmState.HOLD;
	[SerializeField] public ArmState rightArmState =  ArmState.HOLD;

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
		[SerializeField] public MPolygonData spawn;
		[SerializeField] public KeepOrientationEffect.Data orientationData;
		[SerializeField] public KeepPositionEffect.Data positionData;
	}

	[System.Serializable]
	public class ArmData{
		[SerializeField] public JointData shoulder;
		[SerializeField] public JointData joint;
		[SerializeField] public JointData hand;
	}
}


