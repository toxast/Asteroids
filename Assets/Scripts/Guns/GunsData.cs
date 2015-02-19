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
		}

		return null;
	}

	public static BulletGun SimpleGun(Place gp, IPolygonGameObject t)
	{
		GunData d = GunsResources.Instance.guns [0];
		BulletGun gun = new BulletGun(gp, d, t);
		//gun.fireEffect = PositionFireEffect (gp, t, d.fireEffect);
		return gun;
	}


	public static BulletGun SimpleGun2(Place gp, IPolygonGameObject t)
	{
//		BulletGun gun = new BulletGun(gp);
//		gun.transform = t;
//		gun.vertices = PolygonCreator.GetRectShape(0.4f, 0.2f);
//		gun.bulletSpeed = 35f;
//		gun.lifeTime = 2.5f;
//		gun.damage = 3f;
//		gun.fireInterval = 0.5f;
//		gun.color = Color.yellow;
//		gun.fireEffect = PositionFireEffect (gp, t, Singleton<GlobalConfig>.inst.fireEffect2);
		return SimpleGun(gp, t);
	}

	//TODO: missle data
	public static RocketLauncher RocketLauncher(Place gp, IPolygonGameObject t)
	{
		var d = GunsResources.Instance.rocketLaunchers [0];
//		GunData d = new GunData
//		{
//			bulletSpeed = 40f,
//			lifeTime = 5f,
//			damage = 3f,
//			fireInterval = 3f,
//		};
		RocketLauncher gun = new RocketLauncher(gp, d, t);
		//gun.thrusterEffect = Singleton<GlobalConfig>.inst.thrusterEffect;
		//gun.vertices = PolygonCreator.GetRectShape(0.4f, 0.2f);

		//gun.color = Color.yellow;
		//gun.fireEffect = PositionFireEffect (gp, t, Singleton<GlobalConfig>.inst.fireEffect2);
		return gun;
	}

	public static BulletGun TankGun(Place gp, IPolygonGameObject t)
	{
//		BulletGun gun = new BulletGun(gp);
//		gun.transform = t;
//		gun.vertices = PolygonCreator.GetRectShape(0.8f, 0.4f);
//		gun.bulletSpeed = 30f;
//		gun.lifeTime = 2.5f;
//		gun.damage = 3f;
//		gun.fireInterval = 1f;
//		gun.color = Color.yellow;
//		gun.fireEffect = PositionFireEffect (gp, t, Singleton<GlobalConfig>.inst.fireEffect2);
//		return gun;
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
