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
	private float accuracy;
	private float overrideExplosionRadius;

	public RocketLauncher(Place place, RocketLauncherData data, IPolygonGameObject parent):base(place, data.baseData, parent)
	{
		missleParameters = data.missleParameters;
		thrusterEffect = data.thrusterEffect;
		thrusterPos = data.thrusterPos;
		bulletSpeed = data.missleParameters.maxSpeed;
		launchDirection = data.launchDirection;
		launchSpeed = data.launchSpeed;
		accuracy = data.accuracy;
		overrideExplosionRadius = data.overrideExplosionRadius;
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

		Math2d.PositionOnParent (missile.cacheTransform, place, parent.cacheTransform);

		missile.gameObject.name = "missile";
		var controller = new MissileController (missile, missleParameters.maxSpeed, accuracy);
		missile.InitMissile(1f, missleParameters, damage, overrideExplosionRadius, lifeTime);
		missile.destroyOnBoundsTeleport = true;
		missile.destructionType = PolygonGameObject.DestructionType.eJustDestroy;
		missile.SetController (controller);
		missile.targetSystem = new MissileTargetSystem (missile);
		if(launchDirection != Vector2.zero)
		{
			float angle = Math2d.GetRotationRad(missile.cacheTransform.right);
			var byPlace = Math2d.RotateVertex(launchDirection, angle);
			missile.velocity += byPlace.normalized * launchSpeed;
		}
		return missile;
	}

//	protected override void SetBulletTarget (IBullet b)
//	{
//		base.SetBulletTarget (b);
//		if(Main.IsNull(target))
//		{
//			var t = Singleton<Main>.inst.GetNewTarget(b as Missile);
//			if(t != null)
//			{
//				b.SetTarget (t);
//			}
//		}
//		else
//		{
//			b.SetTarget (target);
//		}
//	}

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
