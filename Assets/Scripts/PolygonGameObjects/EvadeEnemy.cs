using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class EvadeEnemy : PolygonGameObject
{
	public static List<Place> gunplaces = new List<Place>
	{
		new Place(new Vector2(2f, 0.0f), new Vector2(1.0f, 0f)),
	};

	public static Vector2[] vertices = PolygonCreator.GetCompleteVertexes(
		new Vector2[]
		{
			new Vector2(2f, -0.25f),
			new Vector2(1.25f, -0.25f),
			new Vector2(1f, -1f),
            new Vector2(0.25f, -1.25f),
            new Vector2(-0.75f, -0.75f),
            new Vector2(-1f, 0f),
		}
		, 1f).ToArray();

	private float movingSpeed = 7f;
	private float minDistanceToTargetSqr = 600;
	private float maxDistanceToTargetSqr = 800;
	private float rangeAngle = 15f; //if angle to target bigger than this - dont even try to shoot
	private float cannonsRotatingSpeed = 55f;

	private bool avoiding = false;
	private Vector2 currentSafePoint;
	private float currentAimAngle = 0;
	private int goRoundTargetSign = 1;

	private List<PolygonGameObject> incomingBullets;
	Rotaitor cannonsRotaitor;

	public void InitEvadeEnemy(PhysicalData physical, List<PolygonGameObject> incomingBullets)
	{
		InitPolygonGameObject (physical);
		this.incomingBullets = incomingBullets;

		cannonsRotaitor = new Rotaitor(cacheTransform, cannonsRotatingSpeed);

		StartCoroutine(Evade());

		StartCoroutine(Aim());

		StartCoroutine(ChangeRotationSign());
	}

	public override void Tick(float delta)
	{
		base.Tick (delta);

		if(Main.IsNull(target))
			return;

		float deltaDist = movingSpeed * delta;

		if(avoiding)
		{
			MoveToSafePoint(deltaDist);
		}
		else
		{
			KeepTargetDistance(deltaDist);
			RotateAroundTarget(deltaDist);
		}

		RotateCannon(delta);

		TickGunsNew (delta);
	}

	private void TickGunsNew(float delta)
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

	private void MoveToSafePoint(float deltaDist)
	{
		Vector2 dist = position - currentSafePoint;
		if(dist.sqrMagnitude < deltaDist*deltaDist)
		{
			position = currentSafePoint;
			//avoiding = false;
		}
		else
		{
			position += (currentSafePoint - position).normalized*deltaDist;
		}
	}

	private void KeepTargetDistance(float deltaDist)
	{
		Vector2 dist = target.position - position;
		float sqrDist = dist.sqrMagnitude;
		if(sqrDist < minDistanceToTargetSqr)
		{
			position -= dist.normalized * deltaDist;
		}
		else if (sqrDist > maxDistanceToTargetSqr)
		{
			position += dist.normalized * deltaDist;
		}
	}

	private void RotateAroundTarget(float deltaDist)
	{
		Vector2 dist = target.position - position;
		Vector2 rotateDirection = new Vector2(dist.y, -dist.x).normalized; //right
		position += rotateDirection * deltaDist * goRoundTargetSign;
	}

	private void RotateCannon(float deltaTime)
	{
		cannonsRotaitor.Rotate(deltaTime, currentAimAngle);
 	}

	IEnumerator Evade()
	{
		while(true)
		{
			EvadeSystem evade = new EvadeSystem(incomingBullets, this);
			avoiding = !evade.safeAtCurrentPosition;
			currentSafePoint = evade.safePosition;
			yield return new WaitForSeconds(0.1f); 
		}
	}

	private IEnumerator Aim()
	{
		float aimInterval = 0.5f;

		while(true)
		{
			if(!Main.IsNull(target))
			{
				AimSystem aim = new AimSystem(target.position, target.velocity, position, guns[0].BulletSpeedForAim);
				if(aim.canShoot)
				{
					currentAimAngle = aim.directionAngleRAD * Mathf.Rad2Deg;
				}
			}
			yield return new WaitForSeconds(aimInterval);
		}
	}

	private IEnumerator ChangeRotationSign()
	{
		while(true)
		{
			float interval = UnityEngine.Random.Range(1f, 10f);
			yield return new WaitForSeconds(interval);
			goRoundTargetSign *= -1;
		}
	}
}
