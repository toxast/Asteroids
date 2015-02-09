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
		return gun;
	}

	//TODO: missle data
	public static RocketLauncher RocketLauncher(GunPlace gp, Transform t)
	{
		RocketLauncher gun = new RocketLauncher(gp);
		gun.transform = t;
		//gun.vertices = PolygonCreator.GetRectShape(0.4f, 0.2f);
		gun.bulletSpeed = 40f;
		gun.lifeTime = 5f;
		gun.damage = 3f;
		gun.fireInterval = 3f;
		gun.color = Color.yellow;
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
		return gun;
	}
	
}
