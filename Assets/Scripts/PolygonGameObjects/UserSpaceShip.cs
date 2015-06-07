using UnityEngine;
using System.Collections;
using System;

public class UserSpaceShip : SpaceShip {

	private bool sentDestroyed = false;
	public event Action destroyed;

	void Start()
	{
		UpdateHealth ();
		UpdateShields ();
	}

	public override void Hit (float dmg)
	{
		base.Hit (dmg);

		if(IsKilled() && !sentDestroyed)
		{
			sentDestroyed = true;
			if(destroyed != null)
				destroyed();
		}

		UpdateHealth ();
		UpdateShields ();
	}

	public override void Tick (float delta)
	{
		base.Tick (delta);
		UpdateShields ();
	}

	private void UpdateShields()
	{
		if (shield != null)
			GameResources.SetShields (shield.currentShields / shield.capacity);
	}

	private void UpdateHealth()
	{
		GameResources.SetHealth (currentHealth/fullHealth);
	}
}
