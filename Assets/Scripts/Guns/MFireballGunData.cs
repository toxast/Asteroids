using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class MFireballGunData : MRocketGunData
{
	[Header ("firebal")]
	public float ballRadius = 3f; 
	public DOTEffect.Data dot;

	public override Gun GetGun(Place place, PolygonGameObject t)
	{
		return new FireballGun(place, this, t);
	}

}

public class FireballGun : RocketLauncher {
	new MFireballGunData data;

	public FireballGun(Place place, MFireballGunData data, PolygonGameObject parent)
		:base(place, data, parent)	{ 
		this.data = data;
	}

	protected override Vector2[] GetVerts ()
	{
		return PolygonCreator.CreatePerfectPolygonVertices(data.ballRadius, 6);
	}

	protected override void InitPolygonGameObject (SpaceShip bullet, PhysicalData ph)
	{
		base.InitPolygonGameObject (bullet, ph);
		bullet.burnDot = data.dot;
	}

	protected override bool CreateExplosion ()	{
		return false;
	}
}

