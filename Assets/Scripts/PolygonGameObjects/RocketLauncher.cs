using UnityEngine;
using System;
using System.Collections;

public class RocketLauncher : Gun
{
	public ParticleSystem thrusterEffect;
	private SpaceshipData missleParameters;
	private Vector2 thrusterPos;
	private Vector2 launchDirection;
	private float launchSpeed;

	public RocketLauncher(Place place, RocketLauncherData data, IPolygonGameObject parent):base(place, data.baseData, parent)
	{
		missleParameters = data.missleParameters;
		thrusterEffect = data.thrusterEffect;
		thrusterPos = data.thrusterPos;
		bulletSpeed = data.missleParameters.maxSpeed;
		launchDirection = data.launchDirection;
		launchSpeed = data.launchSpeed;
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

		if(launchDirection != Vector2.zero)
		{
			float angle = Math2d.GetRotation(missile.cacheTransform.right);
			var byPlace = Math2d.RotateVertex(launchDirection, angle);
			missile.velocity += (Vector3)byPlace.normalized * launchSpeed;
		}
		return missile;
	}

	protected override void SetBulletTarget (IBullet b)
	{
		base.SetBulletTarget (b);
		if(Main.IsNull(target))
		{
			var t = Singleton<Main>.inst.GetNewMissileTarget(b as Missile);
			if(t != null)
			{
				b.SetTarget (t);
			}
		}
		else
		{
			b.SetTarget (target);
		}
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
