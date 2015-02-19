using UnityEngine;
using System;
using System.Collections;

public class RocketLauncher : Gun
{
	public ParticleSystem thrusterEffect;
	private SpaceshipData missleParameters;
	private Vector2 thrusterPos;
	public RocketLauncher(Place place, RocketLauncherData data, IPolygonGameObject parent):base(place, data.baseData, parent)
	{
		missleParameters = data.missleParameters;
		thrusterEffect = data.thrusterEffect;
		thrusterPos = data.thrusterPos;
		bulletSpeed = data.missleParameters.maxSpeed;
	}

	protected override IBullet CreateBullet()
	{
		Missile missile = PolygonCreator.CreatePolygonGOByMassCenter<Missile>(vertices, color);

		if(thrusterEffect != null)
		{
			ParticleSystem e = GameObject.Instantiate(thrusterEffect) as ParticleSystem;
			e.transform.parent = missile.cacheTransform;
			e.transform.localPosition = thrusterPos;//new Vector3(-1,0,-1);
		}

		Math2d.PositionOnShooterPlace (missile.cacheTransform, place, parent.cacheTransform);
		
		missile.gameObject.name = "missile";
		var controller = new MissileController (missile, missleParameters.maxSpeed);
		missile.Init (damage, lifeTime);
		missile.Init (missleParameters);
		missile.SetController (controller);
		missile.SetTarget (target);
		
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
