using UnityEngine;
using System.Collections;

public class SpawnerGun : Gun 
{

	public SpawnerGun(Place place, GunData data, IPolygonGameObject parent):base(place, data, parent)
	{
	}

	protected override IBullet CreateBullet()
	{
		var obj = ObjectsCreator.CreateSpaceShip<EnemySpaceShip> (10);
		Math2d.PositionOnShooterPlace (obj.cacheTransform, place, parent.cacheTransform);
		obj.gameObject.name += "_spawn";
		obj.SetController (new EnemySpaceShipController (obj, new System.Collections.Generic.List<IBullet>(), obj.guns[0].bulletSpeed)); //TODO bullets
		//TODO: Guns Data refactor
		return  new BulletAdapter(obj);
	}

}
