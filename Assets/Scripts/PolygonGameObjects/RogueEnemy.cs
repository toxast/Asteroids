using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RogueEnemy : PolygonGameObject 
{
	public static Vector2[] vertices = PolygonCreator.GetCompleteVertexes(
		new Vector2[]
		{
		new Vector2(0.7f, 0f),
		new Vector2(1f, -0.5f),
		new Vector2(2f, -1f),
		new Vector2(-1, -1),
	}
	, 1f).ToArray();

	public event System.Action<ShootPlace, Transform> FireEvent;

	private float movingSpeed = 6f;
	private float minDistanceToTargetSqr = 600;
	private float maxDistanceToTargetSqr = 800;
	private float rotatingSpeed = 45f;
	private float rangeAngle = 15f;

	float currentAimAngle;

	private SpaceShip target;
	Rotaitor cannonsRotaitor;
	List<ShootPlace> shooters;

	public void Init (SpaceShip ship, List<ShootPlace> shooters) 
	{
		this.target = ship;
		this.shooters = shooters;

		cannonsRotaitor = new Rotaitor(cacheTransform, rotatingSpeed);

		SetAlpha(0f);

		StartCoroutine(FadeAndShoot());
	}

	private Vector2 distToTraget;
	public override void Tick(float delta)
	{
		distToTraget = target.cacheTransform.position - cacheTransform.position;
		
		float deltaDist = movingSpeed * delta;
		
		KeepTargetDistance(deltaDist);
		
		RotateCannon(delta);
	}

	//TODO: refactor from tank enemy and evades
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

	//TODO: refactor from tank enemy and evades
	private void RotateCannon(float deltaTime)
	{
		currentAimAngle = Math2d.GetRotation(ref distToTraget) / Math2d.PIdiv180 ;
		cannonsRotaitor.Rotate(deltaTime, currentAimAngle);
	}

	IEnumerator FadeAndShoot()
	{
		//time to calculate aimAngle in RotateCannon
		yield return new WaitForSeconds(0.3f); 

		float invisibleTime = 2f;
		float fadeInTime = 0.5f;
		float visibleAfterShoot = 1f;
		float fadeOutTime = 1f;
		float deltaTime = 0.2f;

		float ALPHA_1 = 1f;
		float ALPHA_0 = 0f;

		while(true)
		{
			if(Mathf.Abs(cannonsRotaitor.DeltaAngle(currentAimAngle)) < rangeAngle)
			{
				yield return StartCoroutine(FadeTo(ALPHA_1, fadeInTime));

				Fire();

				yield return new WaitForSeconds(visibleAfterShoot); 
				
				yield return StartCoroutine(FadeTo(ALPHA_0, fadeOutTime));
				
				yield return new WaitForSeconds(invisibleTime); 
			}

			yield return new WaitForSeconds(deltaTime); 
		}
	}

	IEnumerator FadeTo(float alpha, float fadeTime)
	{
		float deltaTime = 0.1f;
		float currentAlpha = mesh.colors[0].a;

		bool greater = alpha > currentAlpha;

		float dAlpha = (alpha - currentAlpha);
		
		while(true)
		{
			currentAlpha = mesh.colors[0].a;
			float newAlpha = Mathf.Clamp(currentAlpha + (deltaTime/fadeTime) * dAlpha, 0f, 1f);
			SetAlpha(newAlpha);

			if(greater)
			{
				if(newAlpha >= alpha) break;
			}
			else
			{
				if(newAlpha <= alpha) break;
			}

			yield return new WaitForSeconds(deltaTime); 
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
