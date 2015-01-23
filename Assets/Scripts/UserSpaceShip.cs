using UnityEngine;
using System.Collections;

public class UserSpaceShip : SpaceShip {

	void Start()
	{
		GameResources.SetHealth (health/fullHealth);
	}

	public override void Hit (float dmg)
	{
		base.Hit (dmg);
		GameResources.SetHealth (health/fullHealth);
	}
}
