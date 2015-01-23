using UnityEngine;
using System.Collections;

public class EnemySpaceShip : SpaceShip, IGotTarget
{
	public void SetTarget(PolygonGameObject target)
	{
		(inputController as EnemySpaceShipController).SetTarget (target);
	}
}
