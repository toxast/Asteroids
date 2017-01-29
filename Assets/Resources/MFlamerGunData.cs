using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class MFlamerGunData : MSpreadBulletGunData
{
    public DOTEffect.Data dot;

	public float timeToFullPower = 2.0f;
    public float timeToCool = 0.5f;

	public float range = 5f;

	public override Gun GetGun(Place place, PolygonGameObject t)
	{
		return new FlamerGun(place, this, t);
	}
}
