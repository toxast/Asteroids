using UnityEngine;
using System.Collections;

public class UserSpaceShip : SpaceShip {

	void Start()
	{
		GameResources.SetHealth (currentHealth/fullHealth);
		GameResources.SetShields (currentShields / shieldData.capacity);
	}

	public override void Hit (float dmg)
	{
		base.Hit (dmg);

		GameResources.SetHealth (currentHealth/fullHealth);

		if(shieldData != null)
			GameResources.SetShields (currentShields / shieldData.capacity);
	}

	public override void Tick (float delta)
	{
		base.Tick (delta);

		if(shieldData != null)
			GameResources.SetShields (currentShields / shieldData.capacity);
	}
}
