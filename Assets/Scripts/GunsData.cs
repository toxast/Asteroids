using UnityEngine;
using System.Collections;

public class GunsData 
{

	public static BulletGun SimpleGun(GunPlace gp, Transform t)
	{
		BulletGun gun = new BulletGun(gp);
		gun.transform = t;
		gun.vertices = PolygonCreator.GetRectShape(0.4f, 0.2f);
		gun.bulletSpeed = 35f;
		gun.lifeTime = 2f;
		gun.damage = 3f;
		gun.fireInterval = 0.3f;
		gun.color = Color.red;
		gun.fireEffect = PositionFireEffect (gp, t, Singleton<GlobalConfig>.inst.fireEffect);

		return gun;
	}


	public static BulletGun SimpleGun2(GunPlace gp, Transform t)
	{
		BulletGun gun = new BulletGun(gp);
		gun.transform = t;
		gun.vertices = PolygonCreator.GetRectShape(0.4f, 0.2f);
		gun.bulletSpeed = 35f;
		gun.lifeTime = 2.5f;
		gun.damage = 3f;
		gun.fireInterval = 0.5f;
		gun.color = Color.yellow;
		gun.fireEffect = PositionFireEffect (gp, t, Singleton<GlobalConfig>.inst.fireEffect2);
		return gun;
	}

	//TODO: missle data
	public static RocketLauncher RocketLauncher(GunPlace gp, Transform t)
	{
		RocketLauncher gun = new RocketLauncher(gp);
		gun.thrusterEffect = Singleton<GlobalConfig>.inst.thrusterEffect;
		gun.transform = t;
		//gun.vertices = PolygonCreator.GetRectShape(0.4f, 0.2f);
		gun.bulletSpeed = 40f;
		gun.lifeTime = 5f;
		gun.damage = 3f;
		gun.fireInterval = 3f;
		gun.color = Color.yellow;
		gun.fireEffect = PositionFireEffect (gp, t, Singleton<GlobalConfig>.inst.fireEffect2);
		return gun;
	}

	public static BulletGun TankGun(GunPlace gp, Transform t)
	{
		BulletGun gun = new BulletGun(gp);
		gun.transform = t;
		gun.vertices = PolygonCreator.GetRectShape(0.8f, 0.4f);
		gun.bulletSpeed = 30f;
		gun.lifeTime = 2.5f;
		gun.damage = 3f;
		gun.fireInterval = 1f;
		gun.color = Color.yellow;
		gun.fireEffect = PositionFireEffect (gp, t, Singleton<GlobalConfig>.inst.fireEffect2);
		return gun;
	}

	private static ParticleSystem PositionFireEffect(GunPlace gp, Transform t, ParticleSystem fireEffect)
	{
		var e = GameObject.Instantiate(fireEffect) as ParticleSystem;
		
		float angle = Math2d.GetRotation(gp.dir);
		e.transform.RotateAround(Vector3.zero, Vector3.back, -angle/Math2d.PIdiv180);
		e.transform.position = gp.pos;
		
		angle = Math2d.GetRotation(t.right);
		e.transform.RotateAround(Vector3.zero, Vector3.back, -angle/Math2d.PIdiv180);
		e.transform.position += t.position;
		
		e.transform.parent = t;
        e.transform.position -=  new Vector3(0,0,1);

		return e;
	}
	
}
