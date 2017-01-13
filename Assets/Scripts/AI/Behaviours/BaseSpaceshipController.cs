using UnityEngine;
using System.Collections;

public class BaseSpaceshipController : InputController, IGotTarget
{
    protected PolygonGameObject defendObject;
    protected SpaceShip thisShip;
	protected PolygonGameObject target;

	public bool shooting{ get; protected set; }
	public bool accelerating{ get; protected set; }
	public bool braking{ get; protected set; }
	public Vector2 turnDirection{ get; protected set; }

	public BaseSpaceshipController(SpaceShip thisShip)
	{
		this.thisShip = thisShip;
	}

	public void SetTarget(PolygonGameObject target)
	{
		this.target = target;
	}

    public void SetSpawnParent(PolygonGameObject prnt) {
        defendObject = prnt;
    }

    public void Tick(PolygonGameObject p){}

	protected void Shoot(float accuracy, float bulletsSpeed)
	{
		Vector2 relativeVelocity = (target.velocity);
		AimSystem a = new AimSystem(target.position, accuracy * relativeVelocity - Main.AddShipSpeed2TheBullet(thisShip), thisShip.position, bulletsSpeed);
		if(a.canShoot)
		{
			turnDirection = a.directionDist;
			var angleToRotate = Math2d.ClosestAngleBetweenNormalizedDegAbs (turnDirection.normalized, thisShip.cacheTransform.right);
//			Debug.DrawLine(thisShip.position, thisShip.position + turnDirection*100f, Color.red, 10f);
			this.shooting = (angleToRotate < thisShip.shootAngle);
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

	protected bool logs = false;
	protected void LogWarning(string str) {
		if (logs) {
			Debug.LogWarning (str);
		}
	}
}
