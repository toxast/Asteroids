using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class MSpreadBulletGunData : MGunData
{
    public float spreadAngle;
    public float deceleration;

	public override Gun GetGun(Place place, PolygonGameObject t)
	{
		return new SpreadBulletGun<DeceleratingBullet>(place, this, t);
	}
}
