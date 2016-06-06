using UnityEngine;
using System.Collections;

[System.Serializable]
public class MSpawnerGunData : MGunBaseData
{
	public float bulletSpeed = 35;
	public float fireInterval = 0.5f;
	public ParticleSystem fireEffect;

	public MSpaceshipData spaceshipRef;

	public int maxSpawn = 1;
	public int startSpawn = 1;
	public float startSpawnInterval = 2f;


	public override Gun GetGun(Place place, IPolygonGameObject t)
	{
		return new SpawnerGun(place, this, t);
	}
}
