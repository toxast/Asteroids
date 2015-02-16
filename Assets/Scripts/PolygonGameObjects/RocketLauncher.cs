using UnityEngine;
using System;
using System.Collections;

public class RocketLauncher : Gun
{
	public ParticleSystem thrusterEffect;
	public Color color;
	public RocketLauncher(GunPlace place):base(place){}

	protected override IBullet CreateBullet()
	{
		Missile missile = PolygonCreator.CreatePolygonGOByMassCenter<Missile>(missileVertices, color);

		if(thrusterEffect != null)
		{
			ParticleSystem e = GameObject.Instantiate(thrusterEffect) as ParticleSystem;
			e.transform.parent = missile.cacheTransform;
			e.transform.localPosition = new Vector3(-1,0,-1);
		}

		PositionOnShooterPlace (missile, transform);
		
		missile.gameObject.name = "missile";
		var controller = new MissileController (missile);
		missile.Init ();
		missile.SetController (controller);
		missile.SetTarget (target);
		//missile.Init(target, bulletSpeed, damage, lifeTime); 
		
		return missile;
	}

	public static Vector2[] missileVertices = PolygonCreator.GetCompleteVertexes(
		new Vector2[]
		{
		new Vector2(2f, 0f),
		new Vector2(1.5f, -0.2f),
		new Vector2(0.3f, -0.15f),
		new Vector2(0f, -0.35f),
	}
	, 1f).ToArray();
}
