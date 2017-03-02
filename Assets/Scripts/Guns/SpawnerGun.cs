using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SpawnerGun : GunShooterBase 
{
	public MSpawnDataBase spawnRef; //TODO: not only spaceship
	public int maxSpawn;

	private int startSpawnLeft;
	private float startSpawnInterval;
	private float regularInterval;
	private float spawnFireSpeed;

	private List<PolygonGameObject> spawned = new List<PolygonGameObject>();

	public SpawnerGun(Place place, MSpawnerGunData data, PolygonGameObject parent)
		:base(place, data, parent, 0, 0, data.fireInterval, data.fireEffect)
	{
		this.spawnRef = data.spawnRef;
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
		spawned = spawned.Where (s => !Main.IsNull(s)).ToList ();
		return base.ReadyToShoot () &&  spawned.Count < maxSpawn; //TODO optimize
	}

	protected PolygonGameObject Spawn()
	{
		if (startSpawnLeft > 0)
		{
			startSpawnLeft--;
			if(startSpawnLeft == 0)
			{
				this.fireInterval = regularInterval;
				SetTimeForNextShot();
			}
		}

		var obj = spawnRef.Create(CollisionLayers.GetSpawnedLayer(parent.layerLogic));
        obj.SetSpawnParent(parent);
        obj.gameObject.name += "_spawn";
		obj.reward = 0;

		if (target != null) {
			obj.SetTarget (target);
		}

		Math2d.PositionOnParent (obj.cacheTransform, place, parent.cacheTransform);
		obj.cacheTransform.position += new Vector3 (0, 0, 1);
		obj.velocity += (Vector2)(spawnFireSpeed * obj.cacheTransform.right);
		spawned.Add (obj);
		return obj;
	}

	protected override void Fire()
	{
		var spawn = Spawn ();

		spawn.velocity += Main.AddShipSpeed2TheBullet(parent);

		Singleton<Main>.inst.HandleSpawnFire(spawn);

		if (fireEffect != null)
			fireEffect.Emit (1);
	}
}
