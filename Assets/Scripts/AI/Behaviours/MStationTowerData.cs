using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MStationTowerData : MSpawnDataBase, IGotShape, IGotGuns {
    //public RandomFloat size;	
    //public RandomInt sidesCount;
	public RandomFloat rotationSpeed;
	public float shootAngle = 10f;
	public AccuracyData accuracyData;
    public Color color;
    public PhysicalData physical;
	public DeathData deathData;
	public Vector2[] verts;
	[Header ("Cannons position")]
	[SerializeField] List<MGunSetupData> guns;
	public int cannonsCount;

	public MGunBaseData gun{get{ return guns [0].gun;}}
	public Vector2 firtGunPlace{get{ return guns [0].place.pos;}}

	public Vector2[] iverts {get {return verts;} set{verts = value;}}
	public List<MGunSetupData> iguns {get {return guns;} set{guns = value;}}

	protected override PolygonGameObject CreateInternal(int layer) {
        return ObjectsCreator.CreateStationTower(this, layer);
    }
}
