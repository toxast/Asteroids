using UnityEngine;
using System.Collections;

public class Missile : SpaceShip, IBullet
{
	public float damage{ get; set;}
	protected float lifeTime;
	public bool breakOnDeath { get; set;}

	public void InitMissile(float density, SpaceshipData data, float damage, float lifeTime)
	{
		this.damage = damage; 
		this.lifeTime = lifeTime; 
		InitSpaceShip (density, data);

		DeathAnimation.MakeDeathForThatFellaYo (this, true);
	}

	public override void Tick (float delta)
	{
		base.Tick (delta);
		lifeTime -= delta; 
	}
	
	public bool Expired()
	{
		return lifeTime < 0;
	}


}
