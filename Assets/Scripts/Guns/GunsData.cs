using UnityEngine;
using System.Collections;

public class GunsData 
{

	public static Gun GetGun(MGunSetupData gdata, PolygonGameObject t)
	{
		if (gdata == null) 
		{
			Debug.LogError ("GunPlaceholder " + t.gameObj.name);
			return new GunPlaceholder ();
		}
		else 
		{
			return gdata.GetGun (t);
		}
	}

	public static MGunData SimpleGun()
	{
		return MGunsResources.Instance.guns [0];
	}

	public static MGunData SimpleGun2()
	{
		return SimpleGun();
	}

	public static MRocketGunData RocketLauncher()
	{
		return MGunsResources.Instance.rocketLaunchers [5];
	}

	public static MGunData TankGun()
	{
		return SimpleGun();
	}

	private static ParticleSystem PositionFireEffect(Place gp, Transform trf, ParticleSystem fireEffect)
	{
		var e = GameObject.Instantiate(fireEffect) as ParticleSystem;
		
		float angle = Math2d.GetRotationRad(gp.dir);
		e.transform.RotateAround(Vector3.zero, Vector3.back, -angle*Mathf.Rad2Deg);
		e.transform.position = gp.pos;
		
		angle = Math2d.GetRotationRad(trf.right);
		e.transform.RotateAround(Vector3.zero, Vector3.back, -angle*Mathf.Rad2Deg);
		e.transform.position += trf.position;
		
		e.transform.parent = trf;
        e.transform.position -=  new Vector3(0,0,1);

		return e;
	}
	
}
