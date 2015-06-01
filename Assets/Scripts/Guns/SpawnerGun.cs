using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SpawnerGun : Gun 
{
	public int spaceshipIndex;
	public int maxSpawn;

	private int startSpawnLeft;
	private float startSpawnInterval;
	private float regularInterval;


	private List<BulletAdapter> spawned = new List<BulletAdapter>();

	public SpawnerGun(Place place, SwapnerGunData data, IPolygonGameObject parent):base(place, data.baseData, parent)
	{
		this.spaceshipIndex = data.spaceshipIndex;
		this.maxSpawn = data.maxSpawn;

		this.regularInterval = data.baseData.fireInterval;

		if(data.startSpawn > 0)
			this.fireInterval = data.startSpawnInterval;

		this.startSpawnLeft = data.startSpawn;
	}

	public override bool ReadyToShoot ()
	{
		spawned = spawned.Where (b => !Main.IsNull(b.go)).ToList ();
		return base.ReadyToShoot () &&  spawned.Count < maxSpawn; //TODO optimize
	}

	protected override void SetBulletLayer (IBullet b)
	{
		b.SetCollisionLayerNum(CollisionLayers.GetSpawnedLayer(parent.layer));
	}

	protected override IBullet CreateBullet()
	{
		if (startSpawnLeft > 0)
		{
			startSpawnLeft--;
			if(startSpawnLeft == 0)
			{
				this.fireInterval = regularInterval;
				ResetTime();
			}
		}

		var obj = ObjectsCreator.CreateSpaceShip<SpaceShip> (spaceshipIndex);
		Math2d.PositionOnParent (obj.cacheTransform, place, parent.cacheTransform);
		obj.cacheTransform.position += new Vector3 (0, 0, 1);
		obj.gameObject.name += "_spawn";
//		obj.SetController (new FastSpaceshipAttackController(obj, Singleton<Main>.inst.pBullets, obj.guns[0])); //TODO bullets
		obj.targetSystem = new TargetSystem (obj);
		//TODO: Guns Data refactor
		var adapted =  new BulletAdapter(obj);
		spawned.Add (adapted);
		return adapted;
	}

}
