using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MEarthBossGunData : MGunData {
	public override Gun GetGun (Place place, PolygonGameObject t)
	{
		return new ObjectGun<PolygonGameObject>(place, this, t);
	}
}
