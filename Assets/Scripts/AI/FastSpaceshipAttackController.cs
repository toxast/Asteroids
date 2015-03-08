using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FastSpaceshipAttackController : InputController, IGotTarget
{

	PolygonGameObject thisShip;
	IPolygonGameObject target;
	List<IBullet> bullets;
	bool shooting = false;
	bool accelerating = false;
	bool braking = false;
	Vector2 turnDirection;
	float bulletsSpeed;

	public FastSpaceshipAttackController(PolygonGameObject thisShip, List<IBullet> bullets, float bulletsSpeed)
	{
		this.bulletsSpeed = bulletsSpeed;
		this.bullets = bullets;
		this.thisShip = thisShip;
		thisShip.StartCoroutine (Logic ());
	}
	
	public Vector2 TurnDirection ()
	{
		return turnDirection;
	}
	
	public bool IsShooting()
	{
		return shooting;
	}
	
	public bool IsAccelerating()
	{
		return accelerating;
	}

	public void SetTarget(IPolygonGameObject target)
	{
		this.target = target;
	}
	
	public void Tick(PolygonGameObject p)
	{
		
	}
	
	public bool IsBraking()
	{
		return braking;
	}
	
	private IEnumerator Logic()
	{
		while(true)
		{
			if(!Main.IsNull(target))
			{
				Vector2 dir = target.position - thisShip.position;
				Vector2 dirNorm = dir.normalized;
				float vprojThis = Vector2.Dot(thisShip.velocity, dirNorm); //+ means towards other ship
				float vprojTarget = Vector2.Dot(target.velocity, -dirNorm); //+ means towards other ship
				if(dir.magnitude < thisShip.polygon.R + target.polygon.R + 10 + 1.5*(vprojThis + vprojTarget))
				{
					float evadeSign = Mathf.Sign(Math2d.Cross2(target.velocity, dir));
					var newDir = evadeSign * new Vector2 (-dirNorm.y, dirNorm.x);
					yield return thisShip.StartCoroutine(SetState(newDir, true, false, 2f)); //TODO: by maxV
				}
				else
				{
					turnDirection = dir;
					accelerating = true;
				}
			}
			yield return null;
		}
	}

	private Vector2 RotateDirection(Vector2 dir, float angleMin, float angleMax)
	{
		float angle = UnityEngine.Random.Range (angleMin, angleMax) * Mathf.Sign (UnityEngine.Random.Range (-1f, 1f));
		return Math2d.RotateVertex(dir, angle*Math2d.PIdiv180);
	}

	private IEnumerator SetState(Vector2 dir, bool accelerating, bool shooting, float duration)
	{
		turnDirection = dir;
		this.accelerating = accelerating;
		this.shooting = shooting;
		yield return new WaitForSeconds(duration);
	}
}
