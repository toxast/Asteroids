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

	public void Init(float speed, float damage, float lifeTime)
	{
		this.damage = damage;
		this.lifeTime = lifeTime;
		velocity = cacheTransform.right * speed;
		base.Init (1); //TODO pass
	}


}
 