using UnityEngine;
using System.Collections;

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
	, 2f).ToArray();


	private float movingSpeed = 6f;
	private float minDistanceToTargetSqr = 600;
	private float maxDistanceToTargetSqr = 800;

	private SpaceShip target;

	Rotaitor cannonsRotaitor;

	public void SetTarget(SpaceShip ship)
	{
		this.target = ship;

		float rotatingSpeed = 45f;
		cannonsRotaitor = new Rotaitor(cacheTransform, rotatingSpeed);
	}

	// Use this for initialization
	void Start () 
	{
		Color col = Color.black;
		col.a = 0.2f;
		SetColor(col);

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
		float currentAimAngle = Math2d.GetRotation(ref distToTraget) / Math2d.PIdiv180 ;
		cannonsRotaitor.Rotate(deltaTime, currentAimAngle);
	}

	IEnumerator FadeAndShoot()
	{
		float visibleAfterShoot = 3f;
		float deltaTime = 0.2f;
		while(true)
		{
			yield return StartCoroutine(FadeTo(1f, 1f));
			//TODO: shot
			yield return new WaitForSeconds(visibleAfterShoot); 
			yield return StartCoroutine(FadeTo(0f, 1f));
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


	

}
