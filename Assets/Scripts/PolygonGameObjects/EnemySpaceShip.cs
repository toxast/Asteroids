using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpaceShip : SpaceShip
{
	public List<PolygonGameObject> turrets = new List<PolygonGameObject>();

	public override void SetTarget(IPolygonGameObject target)
	{
		base.SetTarget (target);

		(inputController as IGotTarget).SetTarget (target);

		foreach (var t in turrets)
		{
			IGotTarget gt = t as IGotTarget;
			if(gt != null)
				gt.SetTarget(target);
		}
	}

	public void AddTurret(Vector2 pos, Vector2 dir, PolygonGameObject turret)
	{
		float angle = Math2d.GetRotationG (dir);
		turret.cacheTransform.rotation = Quaternion.Euler(new Vector3(0,0,angle));
		turret.cacheTransform.parent = cacheTransform;
		turret.cacheTransform.localPosition += (Vector3)pos - new Vector3 (0, 0, 1);
		turrets.Add (turret);
	}

	public override void Tick (float delta)
	{
		base.Tick (delta);

		if(!Main.IsNull(target))
		{
			foreach (var t in turrets)
			{
				t.Tick(delta);
			}
		}
	}
}
