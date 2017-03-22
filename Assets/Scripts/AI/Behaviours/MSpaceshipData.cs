using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class MSpaceshipData : MSpawnDataBase, IGotShape, IGotThrusters, IGotGuns, IGotTurrets
{
	public int price = -1;
	public int reward = 0;
	public Color color = Color.white;
	public PhysicalData physical;
	public SpaceshipData mobility;
	public AccuracyData accuracy;
	public ShieldData shield;
	public List<MGunSetupData> guns;
	public List<MyIntList> linkedGuns;
	public List<ParticleSystemsData> thrusters;
	public List<MTurretReferenceData> turrets;
	public Vector2[] verts;
	public int upgradeIndex;
    [SerializeField] public DeathData deathData;

	public Vector2 launchDirection;

	[Space(20)]
	[SerializeField] float explosionRangeCalculated;
	[SerializeField] float explosionDamageCalculated;

    //interfaces
    public Vector2[] iverts {get {return verts;} set{verts = value;}}
	public List<MGunSetupData> iguns {get {return guns;} set{guns = value;}}
	public List<ParticleSystemsData> ithrusters {get {return thrusters;} set{thrusters = value;}}
	public List<MTurretReferenceData> iturrets {get {return turrets;} set{turrets = value;}}

	protected override PolygonGameObject CreateInternal(int layer)
	{
		return ObjectsCreator.CreateSpaceship<SpaceShip>(this, layer);
	} 

	protected virtual void OnValidate(){

		float area;
		Math2d.GetMassCenter (verts, out area);
		explosionRangeCalculated = deathData.overrideExplosionRange >= 0 ? deathData.overrideExplosionRange : DeathAnimation.ExplosionRadius (area);
		explosionDamageCalculated = DeathAnimation.ExplosionDamage (explosionRangeCalculated);

		thrusters.SetDefaultValues ();
	}

	[System.Serializable]
	public class MyIntList{
		public List<int> list;
	}
}

