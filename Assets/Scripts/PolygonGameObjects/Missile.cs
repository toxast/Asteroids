using UnityEngine;
using System.Collections;

public class Missile : SpaceShip, IBullet
{
	public float damage{ get; set;}
	protected float lifeTime;
	public bool breakOnDeath { get; set;}

	public void InitMissile(float density, SpaceshipData data, float damage, float overrideExplosionRadius, float lifeTime)
	{
		this.damage = 0; // rocket will damage only by explosion
		this.overrideExplosionDamage = damage; 
		this.lifeTime = lifeTime; 
		overrideExplosionRange = overrideExplosionRadius;
		PhysicalData ph = new PhysicalData ();
		ph.density = density;
		InitSpaceShip (ph, data);

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
