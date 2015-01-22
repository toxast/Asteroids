using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TowerEnemy : PolygonGameObject
{
	private SpaceShip target;

	private float detectionDistanceSqr;

	private float rotationSpeed = 30f;

	private List<ShootPlace> shooters;
	ShootPlace closestShooter = null;
	float currentAimAngle;
	Rotaitor cannonsRotaitor;
	public event System.Action<ShootPlace, Transform> FireEvent;

	public void Init(SpaceShip target, List<ShootPlace> shooters)
	{
		this.target = target;
		this.shooters = shooters;

		cannonsRotaitor = new Rotaitor (cacheTransform, rotationSpeed);

		StartCoroutine(Aim());
		
		StartCoroutine(FireCoroutine());
	}


	public override void Tick(float delta)
	{
		if (target == null)
			return;
		
		RotateCannon(delta);
	}

	private IEnumerator Aim()
	{
		float aimInterval = 0.5f;

		while(true)
		{
			if(target != null)
			{
				AimSystem aim = new AimSystem(target.cacheTransform.position, target.velocity, cacheTransform.position, shooters[0].speed);
				if(aim.canShoot)
				{
					currentAimAngle = aim.directionAngleRAD / Math2d.PIdiv180;
					float minAngle = 360;

					foreach (var shooter in shooters) 
					{
						float shooterAngle = ShooterAngle(shooter) + transform.eulerAngles.z;
						float dangle = Math2d.DeltaAngleGRAD(currentAimAngle, shooterAngle);

						float absAngle = Mathf.Abs(dangle);
						if(absAngle < minAngle)
						{
							minAngle = absAngle;
							closestShooter = shooter;
						}
					}
					currentAimAngle -= ShooterAngle(closestShooter);
				}
			}
			yield return new WaitForSeconds(aimInterval);
		}
	}

	private float ShooterAngle(ShootPlace p)
	{
		return Math2d.AngleRAD2 (new Vector2 (1, 0), p.position) / Math2d.PIdiv180;
	}

	private void RotateCannon(float deltaTime)
	{
		cannonsRotaitor.Rotate(deltaTime, currentAimAngle);
	}

	private IEnumerator FireCoroutine()
	{
		float defaultInterval = shooters[0].fireInterval;
		while(true)
		{
			yield return new WaitForSeconds(defaultInterval);
			
			if(target != null && closestShooter != null)
			{
				Fire(closestShooter);
			}
		}
	}


	private void Fire(ShootPlace place)
	{
		if(FireEvent != null)
		{
			FireEvent(place, cacheTransform);
		}
	}
}
