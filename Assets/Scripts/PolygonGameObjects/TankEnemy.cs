using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Tank enemy. DEPRECATED See evade enemy
/// </summary>
public class TankEnemy : PolygonGameObject
{
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
	private Vector2 currentSafePoint;

	private Rotaitor cannonsRotaitor;
	private List<IBullet> bullets;

	public void InitAsteroid(List<IBullet> bullets)
	{
		this.bullets = bullets;
		cannonsRotaitor = new Rotaitor(cacheTransform, rotatingSpeed);

		StartCoroutine(Evade());
	}

	protected override float healthModifier {
		get {
			return base.healthModifier * Singleton<GlobalConfig>.inst.TankEnemyHealthModifier;
		}
	}

	
	public override void Tick(float delta)
	{
		base.Tick (delta);

		if(Main.IsNull(target))
			return;

		distToTraget = target.position - position;

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

		TickGuns (delta);
	}
	
	private void MoveToSafePoint(float deltaDist)
	{
		Vector2 dist = position - currentSafePoint;
		if(dist.sqrMagnitude < deltaDist*deltaDist)
		{
			position = currentSafePoint;
			avoiding = false;
		}
		else
		{
			position += (currentSafePoint - position).normalized*deltaDist;
		}
	}
	
	private void KeepTargetDistance(float deltaDist)
	{
		float sqrDist = distToTraget.sqrMagnitude;
		if(sqrDist < minDistanceToTargetSqr)
		{
			position -= distToTraget.normalized * deltaDist;
		}
		else if (sqrDist > maxDistanceToTargetSqr)
		{
			position += distToTraget.normalized * deltaDist;
		}
	}
	
	private void RotateCannon(float deltaTime)
	{
		currentAimAngle = Math2d.GetRotationRad(ref distToTraget)* Mathf.Rad2Deg;
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

	private void TickGuns(float delta)
	{
		for (int i = 0; i < guns.Count; i++) 
		{
			guns[i].Tick(delta);
		}
		
		if(!Main.IsNull(target))
		{
			if(Mathf.Abs(cannonsRotaitor.DeltaAngle(currentAimAngle)) < rangeAngle)
			{
				for (int i = 0; i < guns.Count; i++) 
				{
					guns[i].ShootIfReady();
				}
			}
		}
	}
}
