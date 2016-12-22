using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MTowerData : MSpawnData<SimpleTower>, IGotShape, IGotGuns, IGotTurrets
{
	public int reward;
	public PhysicalData physical;
	public Color color = Color.white;
	public float shootAngle = 20;
	public float rotationSpeed = 50;
	public float repeatTargetCheck = 1f;
	public AccuracyData accuracy;
	public ShieldData shield;
	public List<MGunSetupData> guns;
	public List<int> linkedGuns;
	public List<MTurretReferenceData> turrets;
	public Vector2[] verts;

	//interfaces
	public Vector2[] iverts {get {return verts;} set{verts = value;}}
	public List<MGunSetupData> iguns {get {return guns;} set{guns = value;}}
	public List<MTurretReferenceData> iturrets {get {return turrets;} set{turrets = value;}}

	public override SimpleTower Create()
	{
		return ObjectsCreator.CreateSimpleTower(this);
	}
}
