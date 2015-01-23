using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class TankEnemy : PolygonGameObject, IGotTarget
{
	public event System.Action<ShootPlace, Transform> FireEvent;
	
	public static Vector2[] vertices = PolygonCreator.GetCompleteVertexes(
		new Vector2[]
		{
		new Vector2(1f, -0f),
		new Vector2(0.5f, -0.5f),
		new Vector2(1.5f, -0.5f),
		new Vector2(1.5f, -1f),
		new Vector2(-0.3f, -1f),
	}
	, 2f).ToArray();
	
	private float movingSpeed = 6f;
	private float minDistanceToTargetSqr = 600;
	private float maxDistanceToTargetSqr = 800;
	private float rotatingSpeed = 45f;
	private float rangeAngle = 15f; //if angle to target bigger than this - dont even try to shoot

	private float currentAimAngle = 0;
	private bool avoiding = false;
	private Vector2 distToTraget;
	private Vector3 currentSafePoint;

	private PolygonGameObject target;
	private Rotaitor cannonsRotaitor;
	private List<Bullet> bullets;
	private List<ShootPlace> shooters;

	public void Init(PolygonGameObject ptarget, List<Bullet> bullets, List<ShootPlace> shooters)
	{
		SetTarget(ptarget);
		this.bullets = bullets;
		this.shooters = shooters;

		cannonsRotaitor = new Rotaitor(cacheTransform, rotatingSpeed);

		StartCoroutine(Evade());
		
		StartCoroutine(FireCoroutine());
	}

	protected override float healthModifier {
		get {
			return base.healthModifier * Singleton<GlobalConfig>.inst.TankEnemyHealthModifier;
		}
	}

	public void SetTarget(PolygonGameObject target)
	{
		this.target = target;
	}
	
	public override void Tick(float delta)
	{
		if (target == null)
			return;

		distToTraget = target.cacheTransform.position - cacheTransform.position;

		float deltaDist = movingSpeed * delta;
		
		if(avoiding)
		{
			MoveToSafePoint(deltaDist);
		}
		else
		{
			KeepTargetDistance(deltaDist);
		}
		
		RotateCannon(delta);
	}
	
	private void MoveToSafePoint(float deltaDist)
	{
		Vector2 dist = cacheTransform.position - currentSafePoint;
		if(dist.sqrMagnitude < deltaDist*deltaDist)
		{
			cacheTransform.position = currentSafePoint;
			avoiding = false;
		}
		else
		{
			cacheTransform.position += (currentSafePoint - cacheTransform.position).normalized*deltaDist;
		}
	}
	
	private void KeepTargetDistance(float deltaDist)
	{
		float sqrDist = distToTraget.sqrMagnitude;
		if(sqrDist < minDistanceToTargetSqr)
		{
			cacheTransform.position -= (Vector3) distToTraget.normalized * deltaDist;
		}
		else if (sqrDist > maxDistanceToTargetSqr)
		{
			cacheTransform.position += (Vector3) distToTraget.normalized * deltaDist;
		}
	}
	
	private void RotateCannon(float deltaTime)
	{
		currentAimAngle = Math2d.GetRotation(ref distToTraget) / Math2d.PIdiv180 ;
		cannonsRotaitor.Rotate(deltaTime, currentAimAngle);
	}
	
	IEnumerator Evade()
	{
		while(true)
		{
			EvadeSystem evade = new EvadeSystem(bullets, this);
			avoiding = !evade.safeAtCurrentPosition;
			currentSafePoint = evade.safePosition;
			yield return new WaitForSeconds(0.1f); 
		}
	}
	
	private IEnumerator FireCoroutine()
	{
		while(true)
		{
			yield return new WaitForSeconds(shooters[0].fireInterval);

			if(target != null)
			{
				if(Mathf.Abs(cannonsRotaitor.DeltaAngle(currentAimAngle)) < rangeAngle)
				{
					Fire();
				}
			}
		}
	}
	
	private void Fire()
	{
		if(FireEvent == null)
		{
			return;
		}

		foreach(var shooter in shooters)
		{
			FireEvent(shooter, cacheTransform);
		}
	}
	
	
}
