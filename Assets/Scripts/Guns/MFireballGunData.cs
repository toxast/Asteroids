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







