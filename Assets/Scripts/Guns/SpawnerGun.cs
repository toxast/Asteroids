using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SpawnerGun : GunShooterBase 
{
	public MSpaceshipData spaceshipRef;
	public int maxSpawn;

	private int startSpawnLeft;
	private float startSpawnInterval;
	private float regularInterval;
	private float spawnFireSpeed;

	private List<BulletAdapter> spawned = new List<BulletAdapter>();

	public SpawnerGun(Place place, MSpawnerGunData data, IPolygonGameObject parent)
		:base(place, data, parent, 0, 0, data.fireInterval, data.fireEffect)
	{
		this.spaceshipRef = data.spaceshipRef;
		this.maxSpawn = data.maxSpawn;
		this.spawnFireSpeed = data.bulletSpeed;
		this.regularInterval = data.fireInterval;

		if(data.startSpawn > 0)
			this.fireInterval = data.startSpawnInterval;

		this.startSpawnLeft = data.startSpawn;
	}

	public override float BulletSpeedForAim{ get { return Mathf.Infinity; } }

	public override float Range
	{
		get{return Mathf.Infinity;}
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

		var obj = ObjectsCreator.MCreateSpaceShip<SpaceShip> (spaceshipRef);
		obj.reward = 0;

		if(target != null)
			obj.SetTarget (target);

		Math2d.PositionOnParent (obj.cacheTransform, place, parent.cacheTransform);
		obj.cacheTransform.position += new Vector3 (0, 0, 1);
		obj.gameObject.name += "_spawn";
		obj.velocity += (Vector2)(spawnFireSpeed * obj.cacheTransform.right);
		obj.targetSystem = new TargetSystem (obj);
		var adapted =  new BulletAdapter(obj);
		spawned.Add (adapted);
		return adapted;
	}
}
