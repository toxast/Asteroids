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
		UpdateHealth ();
		UpdateShields ();
	}

	public override void Tick (float delta)
	{
		base.Tick (delta);
		UpdateShields ();
	}

	public override void Heal (float amount)
	{
		base.Heal (amount);
		UpdateHealth ();
	}

	private void UpdateShields()
	{
        if (shield != null && shield.capacity > 0) {
            GameResources.SetShields(shield.currentShields / shield.capacity);
        } else {
            GameResources.SetShields(0);
        }
	}

	private void UpdateHealth()
	{
        if (fullHealth > 0) {
            GameResources.SetHealth(currentHealth / fullHealth);
        } else {
            GameResources.SetHealth(0);
        }
	}

	protected override bool RestrictShootingByFastRotation{get{ return false; } }

	public override void HandleDestroy ()
	{
		base.HandleDestroy ();
		if(!sentDestroyed)
		{
			sentDestroyed = true;
			if(destroyed != null)
				destroyed();
		}
	}
}
