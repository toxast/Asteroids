using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MTowerData : MSpawnDataBase, IGotShape, IGotGuns, IGotTurrets
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
	public List<List<int>> linkedGuns;
	public List<MTurretReferenceData> turrets;
	public Vector2[] verts;

    //interfaces
    public Vector2[] iverts {get {return verts;} set{verts = value;}}
	public List<MGunSetupData> iguns {get {return guns;} set{guns = value;}}
	public List<MTurretReferenceData> iturrets {get {return turrets;} set{turrets = value;}}

	protected override PolygonGameObject CreateInternal(int layer)
	{
		return ObjectsCreator.CreateSimpleTower(this, layer);
	}
}
