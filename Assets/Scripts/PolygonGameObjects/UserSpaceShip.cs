using UnityEngine;
using System.Collections;

public class UserSpaceShip : SpaceShip {

	void Start()
	{
		UpdateHealth ();
		UpdateShields ();
	}

	public override void Hit (float dmg)
	{
		base.Hit (dmg);

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
