﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpaceShip : SpaceShip, IGotTarget
{
	public List<PolygonGameObject> turrets = new List<PolygonGameObject>();

	public void SetTarget(PolygonGameObject target)
	{
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

		foreach (var t in turrets)
		{
			t.Tick(delta);
		}
	}
}