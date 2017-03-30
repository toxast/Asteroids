using UnityEngine;
using System.Collections;

[System.Serializable]
public class MSpawnerGunData : MGunBaseData
{
	public float bulletSpeed = 35;
	public float fireInterval = 0.5f;
	public ParticleSystem fireEffect;

	public MSpawnDataBase spawnRef;

	public int maxSpawn = 1;
	public int startSpawn = 1;
	public float startSpawnInterval = 2f;

	[Header ("parent destruction")]
	public bool killOnParentDestroyed = false;
	public bool disableExplosion = false;


	public override Gun GetGun(Place place, PolygonGameObject t)
	{
		return new SpawnerGun(place, this, t);
	}
}
