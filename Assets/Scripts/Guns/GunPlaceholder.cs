using UnityEngine;
using System.Collections;

public class GunPlaceholder : Gun
{
	public GunPlaceholder () : base (null, null, null)
	{}

	public override float BulletSpeedForAim {
		get {
			return Mathf.Infinity;
		}
	}

	public override void SetTimeForNexShot ()
	{
		
	}

	public override bool ReadyToShoot()
	{
		return false;
	}

	public override void ShootIfReady()
	{
		
	}
}
