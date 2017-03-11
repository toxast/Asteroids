using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TowerEnemy : PolygonGameObject
{
	float detectionDistanceSqr;
	float rotationSpeed;
    Rotaitor cannonsRotaitor;

    //changable
    Gun closestGun = null;
    float currentAimAngle;

    public void Init(MStationTowerData data) {
        rotationSpeed = data.rotationSpeed.RandomValue;
        InitPolygonGameObject(data.physical); 
		cannonsRotaitor = new Rotaitor (cacheTransform, rotationSpeed);
		StartCoroutine(Aim());
	}

	public override void Tick(float delta) {
		base.Tick (delta);
		Brake (delta, 5f);
		RotateCannon(delta);
		TickGunsNew (delta);
	}

	float lastDeltaAngle = 360f;
	private IEnumerator Aim() {
		float aimInterval = 0.5f;

		while (true) {
			if (!Main.IsNull (target)) {
				AimSystem aim = new AimSystem (target.position, target.velocity, position, guns [0].BulletSpeedForAim);
				if (!aim.canShoot) {
					aim = new AimSystem (target.position, target.velocity, position, 1.2f * target.velocity.magnitude);
				}
				if (aim.canShoot) {
					currentAimAngle = aim.directionAngleRAD * Mathf.Rad2Deg;
					float minAngle = 360;
					foreach (var gun in guns) {
						if (!gun.ReadyToShoot ()) {
							continue;
						}
						float shooterAngle = GunAngle (gun) + transform.eulerAngles.z;
						float dangle = Math2d.DeltaAngleDeg (currentAimAngle, shooterAngle);
						float absAngle = Mathf.Abs (dangle);
						if (absAngle < minAngle) {
							minAngle = absAngle;
							closestGun = gun;
						}
					}
					lastDeltaAngle = minAngle;
					currentAimAngle -= GunAngle (closestGun);
				}
			}
			yield return new WaitForSeconds (aimInterval);
		}
	}

	private float GunAngle(Gun p) {
		return Math2d.AngleRad (new Vector2 (1, 0), p.place.pos) * Mathf.Rad2Deg;
	}

	private void RotateCannon(float deltaTime) {
		cannonsRotaitor.Rotate (deltaTime, currentAimAngle);
	}

	private void TickGunsNew(float delta) {
		//base.TickGuns ();
		//base.Shoot ();
		for (int i = 0; i < guns.Count; i++) {
			guns [i].Tick (delta);
		}
		if (target != null && closestGun != null && lastDeltaAngle < 10f) {
			closestGun.ShootIfReady ();
		}
	}

}
