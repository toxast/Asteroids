using UnityEngine;
using System;
using System.Collections;
using System.Linq;

public class SimpleTowerRotating : PolygonGameObject, IFreezble
{
	MTowerRotatingData data;

	private Vector2 aimDirNorm;

	AdvancedTurnComponent cannonsRotaitorCurrent;
	AdvancedTurnComponent cannonsRotaitorReload;
	AdvancedTurnComponent cannonsRotaitorShoot;

	protected AIHelper.AccuracyChangerAdvanced accuracyChanger;
	protected float accuracy { get { return accuracyChanger.accuracy; } }


	public void InitTowerRotating (MTowerRotatingData data) {
		this.data = data;
		InitPolygonGameObject (data.physical);

		accuracyChanger = new AIHelper.AccuracyChangerAdvanced(data.accuracy, this);
		cannonsRotaitorReload = new AdvancedTurnComponent(this, data.rotationSpeed);
		cannonsRotaitorShoot = new AdvancedTurnComponent(this, data.rotationSpeedWhileShooting);
		cannonsRotaitorCurrent = cannonsRotaitorReload;
		StartCoroutine (FiringRoutine ());
		aimDirNorm = cacheTransform.right;
	}

	public override void Freeze(float multipiler){
		base.Freeze (multipiler);
		cannonsRotaitorReload.Freeze(multipiler);
		cannonsRotaitorShoot.Freeze (multipiler);
	}

	//float lastDelta = 0;
	public override void Tick(float delta) 	{
	//	lastDelta = delta;
		base.Tick (delta);
		accuracyChanger.Tick (delta);
		Brake (delta, 4f);
		CalculateAim ();
		TickGuns (delta);
	}

	protected override void ApplyRotation (float dtime)	{
		cannonsRotaitorCurrent.TurnByDirection (aimDirNorm, dtime);
	}

	IEnumerator FiringRoutine(){
		while (true) {
			yield return null;
			while (cannonsRotaitorCurrent.isFastRotation()) {
				yield return null;
			}

			bool gunsInPosition = false;
			bool gunsAreReady = false;
			while (!(gunsInPosition && gunsAreReady && TargetNotNull)) {
				CalculateAim();
				gunsInPosition = cannonsRotaitorCurrent.inAngleRange (aimDirNorm, 5f);
				gunsAreReady = guns.TrueForAll (g => g.ReadyToShoot ());
				yield return null;
			}

			bool rotateLeft = TargetNotNull && cannonsRotaitorCurrent.IsDirectionToTheLeft (target.position - position);
			cannonsRotaitorCurrent.TurnByDirection (cacheTransform.right, Time.deltaTime); //to reduce possible rotation
			cannonsRotaitorCurrent = cannonsRotaitorShoot;
			Shoot ();
			yield return null;
			while (guns.Exists (g => g.IsFiring ())) {
				aimDirNorm = Math2d.MakeRight (cacheTransform.right);
				if (rotateLeft) {
					aimDirNorm = -aimDirNorm;
				}
				yield return null;
			}
			cannonsRotaitorCurrent.TurnByDirection (cacheTransform.right, Time.deltaTime); //to reduce possible rotation
			cannonsRotaitorCurrent = cannonsRotaitorReload;
		}
	}



	private void CalculateAim()
	{
		if (TargetNotNull) {
			AimSystem aim = new AimSystem (target.position, accuracy * target.velocity, position, guns [0].BulletSpeedForAim);
			if (aim.canShoot) {
				aimDirNorm = aim.directionDist.normalized;
			}
		} else {
			aimDirNorm = cacheTransform.right.normalized;
		}

        var shootAngle = data.shootAngle;
		var dirLeft = Math2d.RotateVertexDeg (aimDirNorm, shootAngle);
		var dirRight = Math2d.RotateVertexDeg (aimDirNorm, -shootAngle);
		if (Math2d.DegBetweenNormUnsigned (dirLeft, cacheTransform.right) > Math2d.DegBetweenNormUnsigned (dirRight, cacheTransform.right)) {
			aimDirNorm = dirRight;
		} else {
			aimDirNorm = dirLeft;
		}
	}
}
