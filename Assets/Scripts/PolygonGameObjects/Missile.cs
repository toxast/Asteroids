using UnityEngine;
using System.Collections;

public class Missile : SpaceShip, IBullet
{
	public float damage{ get; set;}
	protected float lifeTime;
	public bool breakOnDeath { get; set;}

	public void Init(float damage, float lifeTime)
	{
		DeathAnimation.MakeDeathForThatFellaYo (this, true);
		this.damage = damage; 
		this.lifeTime = lifeTime; 
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
