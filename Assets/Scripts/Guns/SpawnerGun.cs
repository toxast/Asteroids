using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SpawnerGun : GunShooterBase 
{
	public MSpawnDataBase spawnRef; 
	public int maxSpawn;
	MSpawnerGunData data;
	private int startSpawnLeft;
	private float startSpawnInterval;
	private float regularInterval;
	private float spawnFireSpeed;

	private List<PolygonGameObject> spawned = new List<PolygonGameObject>();

	public SpawnerGun(Place place, MSpawnerGunData data, PolygonGameObject parent)
		:base(place, data, parent, 0, 0, data.fireInterval, data.fireEffect)
	{
		this.data = data;
		if (data.startSpawn > 0) {
			this.fireInterval = data.startSpawnInterval;
		}
		this.startSpawnLeft = data.startSpawn;

		if (data.killOnParentDestroyed) {
			parent.OnDestroying += OnParentDestroyed;
		}
	}

	void OnParentDestroyed() {
		for (int i = 0; i < spawned.Count; i++) {
			var sp = spawned [i];
			if (!Main.IsNull (sp)) {
				if(data.disableExplosion){
					sp.deathAnimation = null;
				}
				sp.Kill ();
			}
		}
	}

	public override float BulletSpeedForAim{ get { return Mathf.Infinity; } }

	public override float Range
	{
		get{return Mathf.Infinity;}
	}

	public override bool ReadyToShoot ()
	{
		spawned = spawned.Where (s => !Main.IsNull(s)).ToList ();
		return base.ReadyToShoot () &&  spawned.Count < data.maxSpawn; //TODO optimize
	}

	protected PolygonGameObject Spawn()
	{
		if (startSpawnLeft > 0)
		{
			startSpawnLeft--;
			if(startSpawnLeft == 0)
			{
				this.fireInterval = data.fireInterval;
				SetTimeForNextShot();
			}
		}

		var obj = data.spawnRef.Create(CollisionLayers.GetSpawnedLayer(parent.layerLogic));
        obj.SetSpawnParent(parent);
        obj.gameObject.name += "_spawn";
		obj.reward = 0;
		obj.priorityMultiplier = 0.5f;

		if (target != null) {
			obj.SetTarget (target);
		}

		Math2d.PositionOnParent (obj.cacheTransform, place, parent.cacheTransform);
		obj.cacheTransform.position += new Vector3 (0, 0, 1);
		obj.velocity += (Vector2)(data.bulletSpeed * obj.cacheTransform.right);
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
