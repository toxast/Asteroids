using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SpawnerGun : Gun 
{
	public int spaceshipIndex;
	public int maxSpawn;

	private List<BulletAdapter> spawned = new List<BulletAdapter>();

	public SpawnerGun(Place place, SwapnerGunData data, IPolygonGameObject parent):base(place, data.baseData, parent)
	{
		this.spaceshipIndex = data.spaceshipIndex;
		this.maxSpawn = data.maxSpawn;
	}

	public override bool ReadyToShoot ()
	{
		spawned = spawned.Where (b => !Main.IsNull(b.go)).ToList ();
		return base.ReadyToShoot () &&  spawned.Count < maxSpawn; //TODO optimize
	}

	protected override void SetBulletLayer (IBullet b)
	{
		if(parent.layer == (int)GlobalConfig.eLayer.USER || parent.layer == (int)GlobalConfig.eLayer.TEAM_USER)
		{
			b.SetCollisionLayerNum(GlobalConfig.ilayerTeamUser);
		}
		else if(parent.layer == (int)GlobalConfig.eLayer.TEAM_ENEMIES)
		{
			b.SetCollisionLayerNum(GlobalConfig.ilayerTeamEnemies);
		}
	}

	protected override IBullet CreateBullet()
	{
		var obj = ObjectsCreator.CreateSpaceShip<SpaceShip> (spaceshipIndex);
		Math2d.PositionOnShooterPlace (obj.cacheTransform, place, parent.cacheTransform);
		obj.gameObject.name += "_spawn";
		obj.SetController (new FastSpaceshipAttackController(obj, Singleton<Main>.inst.pBullets, obj.guns[0])); //TODO bullets
		obj.targetSystem = new TargetSystem (obj);
		//TODO: Guns Data refactor
		var adapted =  new BulletAdapter(obj);
		spawned.Add (adapted);
		return adapted;
	}

}
