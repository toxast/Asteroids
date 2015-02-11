using UnityEngine;
using System.Collections;

public class BulletBase : PolygonGameObject
{
	public float damage;
	protected float lifeTime;


	public virtual void Init(float damage, float lifeTime)
	{
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
