using UnityEngine;
using System.Collections;

public class BaseSpaceshipController : InputController, IGotTarget
{
    protected PolygonGameObject defendObject;
    protected SpaceShip thisShip;
	protected PolygonGameObject target;

	public bool shooting{ get; protected set; }
	public bool accelerating{ get; protected set; }
	public float accelerateValue01{ get{ return 1f;}} 
	public bool braking{ get; protected set; }
	public Vector2 turnDirection{ get; protected set; }
    AIHelper.AccuracyChangerAdvanced accuracyChanger;
    protected float accuracy { get { return accuracyChanger.accuracy; } }

    public virtual void Freeze(float m){ }

	public BaseSpaceshipController(SpaceShip thisShip, AccuracyData accuracyData) {
		this.thisShip = thisShip;
        accuracyChanger = new AIHelper.AccuracyChangerAdvanced(accuracyData, thisShip);
    }

	public void SetTarget(PolygonGameObject target)
	{
		this.target = target;
	}

    public void SetSpawnParent(PolygonGameObject prnt) {
        defendObject = prnt;
    }

    public virtual void Tick(float delta) {
        accuracyChanger.Tick(delta);
    }

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
		if (!action) {
			timeLeft -= Time.deltaTime;
		}

		if(timeLeft < 0)
		{
			timeLeft = UnityEngine.Random.Range (min, max);
			action = true;
		}
	}

	protected IEnumerator SetFlyDir(Vector2 dir, float duration, bool accelerating = true, bool shooting = false)
	{
		Debug.LogWarning ("SetFlyDir " + dir + " " + duration);
		turnDirection = dir;
		SetAcceleration (accelerating);
		this.shooting = shooting;
		duration = Mathf.Max (0, duration);
		yield return new WaitForSeconds(duration);
	}

	protected IEnumerator CowardAction(AIHelper.Data tickData, int turnsTotal, float approhimateDuration = 3){
		Debug.LogError ("coward action " + turnsTotal);
		int turns = turnsTotal;
		while (turns > 0) {
			turns--;
			float duration = approhimateDuration / turnsTotal + UnityEngine.Random.Range (-0.3f, 0.5f);
			float angle = UnityEngine.Random.Range (120f, 180f);
			var newDir = Math2d.RotateVertexDeg (tickData.dirNorm, tickData.evadeSign * angle);
			yield return thisShip.StartCoroutine (SetFlyDir (newDir, duration)); 
			tickData.Refresh (thisShip, target);
		}
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
