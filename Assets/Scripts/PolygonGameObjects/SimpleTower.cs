using UnityEngine;
using System.Collections;

public class SimpleTower : PolygonGameObject, IGotTarget
{

	public static Vector2[] vertices = PolygonCreator.CreateTowerVertices2(1, 6);
	
	public event System.Action<ShootPlace, Transform> FireEvent;
	
	private float rangeAngle = 0.5f; //if angle to target bigger than this - dont even try to shoot
	private float cannonsRotatingSpeed = 55f;
	
	private float currentAimAngle = 0;
	
	private PolygonGameObject target;
	Rotaitor cannonsRotaitor;
	ShootPlace shooter;
	
	public void Init(ShootPlace shooter)
	{
		this.shooter = shooter;
		
		cannonsRotaitor = new Rotaitor(cacheTransform, cannonsRotatingSpeed);
		
		StartCoroutine(Aim());
		
		StartCoroutine(FireCoroutine());
	}
	
	public void SetTarget(PolygonGameObject target)
	{
		this.target = target;
	}
	
	public override void Tick(float delta)
	{
		base.Tick (delta);
		
		if (target == null)
			return;
		
		RotateCannon(delta);
	}
	
	private void RotateCannon(float deltaTime)
	{
		cannonsRotaitor.Rotate(deltaTime, currentAimAngle);
	}
	
	
	private IEnumerator Aim()
	{
		float aimInterval = 0.5f;
		
		while(true)
		{
			if(target != null)
			{
				AimSystem aim = new AimSystem(target.cacheTransform.position, target.velocity, cacheTransform.position, shooter.speed);
				if(aim.canShoot)
				{
					currentAimAngle = aim.directionAngleRAD / Math2d.PIdiv180;
				}
			}
			yield return new WaitForSeconds(aimInterval);
		}
	}
	
	private IEnumerator FireCoroutine()
	{
		float defaultInterval = shooter.fireInterval;
		float shortInterval = defaultInterval/2f;
		
		float deltaTime = defaultInterval;
		while(true)
		{
			yield return new WaitForSeconds(deltaTime);
			
			if(target != null)
			{
				if(Mathf.Abs(cannonsRotaitor.DeltaAngle(currentAimAngle)) < rangeAngle)
				{
					Fire();
					deltaTime = defaultInterval;
				}
				else
				{
					deltaTime = shortInterval;
				}
			}
		}
	}
	
	private void Fire()
	{
		if(FireEvent != null)
		{
			FireEvent(shooter, cacheTransform);
		}
	}
}
