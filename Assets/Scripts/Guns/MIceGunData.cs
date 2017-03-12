using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MIceGunData : MRocketGunData
{
	[Header ("ice")]
	public IceEffect.Data iceData;

	public override Gun GetGun(Place place, PolygonGameObject t)
	{
		return new IceGun(place, this, t);
	}

}
