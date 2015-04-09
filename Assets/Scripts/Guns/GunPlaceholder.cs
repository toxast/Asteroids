using UnityEngine;
using System.Collections;

public class GunPlaceholder : Gun
{
	protected override void Fire (IBullet b)
	{
	}

	public override void Tick (float delta)
	{
	}

	public override float Range {
		get {
			return 0;
		}
	}

	protected override IBullet CreateBullet ()
	{
		return null;
	}

	public override bool ReadyToShoot ()
	{
		return false;
	}
}
