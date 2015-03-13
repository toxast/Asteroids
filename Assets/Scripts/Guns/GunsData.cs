using UnityEngine;
using System.Collections;

public class GunsData 
{

	public static Gun GetGun(GunSetupData gdata, IPolygonGameObject t)
	{
		switch (gdata.type) 
		{
			case GunSetupData.eGuns.BULLET:
				 var gun1 = GunsResources.Instance.guns[gdata.index];
				 return new BulletGun(gdata.place, gun1, t);

			case GunSetupData.eGuns.ROCKET:
				var gun2 = GunsResources.Instance.rocketLaunchers[gdata.index];
				return new RocketLauncher(gdata.place, gun2, t);

			case GunSetupData.eGuns.SPAWNER:
				var gun3 = GunsResources.Instance.spawnerGuns[gdata.index];
				return new SpawnerGun(gdata.place, gun3, t);
		}

		return null;
	}

	public static BulletGun SimpleGun(Place gp, IPolygonGameObject t)
	{
		GunData d = GunsResources.Instance.guns [0];
		BulletGun gun = new BulletGun(gp, d, t);
		return gun;
	}

	public static BulletGun SimpleGun2(Place gp, IPolygonGameObject t)
	{
		return SimpleGun(gp, t);
	}

	public static RocketLauncher RocketLauncher(Place gp, IPolygonGameObject t)
	{
		var d = GunsResources.Instance.rocketLaunchers [5];
		RocketLauncher gun = new RocketLauncher(gp, d, t);
		return gun;
	}

	public static BulletGun TankGun(Place gp, IPolygonGameObject t)
	{
		return SimpleGun(gp, t);
	}

	private static ParticleSystem PositionFireEffect(Place gp, Transform trf, ParticleSystem fireEffect)
	{
		var e = GameObject.Instantiate(fireEffect) as ParticleSystem;
		
		float angle = Math2d.GetRotation(gp.dir);
		e.transform.RotateAround(Vector3.zero, Vector3.back, -angle/Math2d.PIdiv180);
		e.transform.position = gp.pos;
		
		angle = Math2d.GetRotation(trf.right);
		e.transform.RotateAround(Vector3.zero, Vector3.back, -angle/Math2d.PIdiv180);
		e.transform.position += trf.position;
		
		e.transform.parent = trf;
        e.transform.position -=  new Vector3(0,0,1);

		return e;
	}
	
}
