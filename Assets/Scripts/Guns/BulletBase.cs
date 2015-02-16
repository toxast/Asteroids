using UnityEngine;
using System.Collections;

public class BulletBase
{
	public float damage{ get; set;}
	protected float lifeTime;


	public virtual void Init(float damage, float lifeTime)
	{
		this.damage = damage;
		this.lifeTime = lifeTime;
	}

	public void Tick (float delta)
	{
		lifeTime -= delta; 
	}


	public bool Expired()
	{
		return lifeTime < 0;
	}

}
