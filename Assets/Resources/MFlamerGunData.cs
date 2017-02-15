using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[SerializeField]
public class MFlamerGunData : MGunData
{
    public DOTEffect.Data dot;
	public RandomFloat deceleration;
	public float velocityRandomRange = 5;

	public override Gun GetGun(Place place, PolygonGameObject t)
	{
		return new FlamerGun(place, this, t);
	}
}
