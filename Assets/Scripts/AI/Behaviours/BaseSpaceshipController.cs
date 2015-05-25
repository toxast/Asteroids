using UnityEngine;
using System.Collections;

public class BaseSpaceshipController : InputController, IGotTarget
{
	protected SpaceShip thisShip;
	protected IPolygonGameObject target;

	public bool shooting{ get; protected set; }
	public bool accelerating{ get; protected set; }
	public bool braking{ get; protected set; }
	public Vector2 turnDirection{ get; protected set; }

	public BaseSpaceshipController(SpaceShip thisShip)
	{
		this.thisShip = thisShip;
	}

	public void SetTarget(IPolygonGameObject target)
	{
		this.target = target;
	}

	public void Tick(PolygonGameObject p){}

	protected void Shoot(float accuracy, float bulletsSpeed)
	{
		Vector2 relativeVelocity = (target.velocity - Main.AddShipSpeed2TheBullet(thisShip));
		AimSystem a = new AimSystem(target.position, accuracy * relativeVelocity, thisShip.position, bulletsSpeed);
		if(a.canShoot)
		{
			turnDirection = a.direction;
			this.shooting = (Math2d.ClosestAngleBetweenNormalizedDegAbs(turnDirection.normalized, thisShip.cacheTransform.right) < 15f);
		}
		else
		{
			turnDirection = target.position - thisShip.position;
			this.shooting = false;
		}
	}

	protected void TickActionVariable(ref bool action, ref float timeLeft, float min, float max)
	{
		if(!action)
			timeLeft -= Time.deltaTime;
		
		if(timeLeft < 0)
		{
			timeLeft = UnityEngine.Random.Range (min, max);
			action = true;
		}
	}

	protected IEnumerator SetFlyDir(Vector2 dir, float duration, bool accelerating = true, bool shooting = false)
	{
		turnDirection = dir;
		SetAcceleration (accelerating);
		this.shooting = shooting;
		yield return new WaitForSeconds(duration);
	}

	protected void SetAcceleration(bool accelrate)
	{
		//Debug.LogWarning ("accelerating " + accelrate);
		accelerating = accelrate;
		
		if (accelerating)
			braking = false;
	}
	
	protected void Brake()
	{
		accelerating = false;
		braking = true;
	}
}
