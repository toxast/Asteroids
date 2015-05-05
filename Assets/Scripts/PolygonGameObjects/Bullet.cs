using UnityEngine;
using System.Collections;

public class Bullet : PolygonGameObject, IBullet 
{
	private float startingSpeed;
	public float damage{ get; set;}
	protected float lifeTime;
	public bool breakOnDeath { get; set;}

	public override void Tick (float delta)
	{
		base.Tick (delta);
		lifeTime -= delta; 
	}
	
	public bool Expired()
	{
		return lifeTime < 0;
	}

	public void InitBullet(float speed, float damage, float lifeTime)
	{
		base.InitPolygonGameObject (1); //TODO pass
		this.damage = damage;
		this.lifeTime = lifeTime;
		velocity = cacheTransform.right * speed;
	}


}
 