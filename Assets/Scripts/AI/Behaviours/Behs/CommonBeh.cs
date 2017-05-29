using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public abstract class CommonBeh : BaseBeh {
	public class Data {
		public SpaceShip thisShip;
		public AIHelper.AccuracyChangerAdvanced accuracyChanger;
		public Func<AIHelper.Data> getTickData;
		public float comformDistanceMin;
		public float comformDistanceMax;
		public Gun mainGun;
	}

	protected Data data;
	protected SpaceShip thisShip{get{return data.thisShip;}}
	protected PolygonGameObject target{ get{ return thisShip.target; }}
	protected float accuracy{ get{ return data.accuracyChanger.accuracy; }}

	protected bool TargetNULL(){
		return Main.IsNull(target);
	}

	public CommonBeh (Data data){
		this.data = data; 
	}

	protected virtual float SelfSpeedAccuracy(){
		return 1;
	}

	protected void Shoot(float accuracy, float bulletsSpeed)
	{
		if (Main.IsNull (target)) {
			return;
		}
		Vector2 turnDirection;
		bool shooting;
		Vector2 relativeVelocity = (target.velocity);
		AimSystem a = new AimSystem(target.position, accuracy * relativeVelocity - SelfSpeedAccuracy() * Main.AddShipSpeed2TheBullet(thisShip), thisShip.position, bulletsSpeed);
		if(a.canShoot) {
			turnDirection = a.directionDist;
			var angleToRotate = Math2d.ClosestAngleBetweenNormalizedDegAbs (turnDirection.normalized, thisShip.cacheTransform.right);
			shooting = (angleToRotate < thisShip.shootAngle);
		} else {
			turnDirection = target.position - thisShip.position;
			shooting = false;
		}
		FireDirChange (turnDirection);
		FireShootChange (shooting);
	}


	protected IEnumerator CowardAction (float approhimateDuration, int turnsTotal) {
		//int turnsTotal = UnityEngine.Random.Range (2, 5);
		int turns = turnsTotal;
		while (turns > 0) {
			turns--;
			float duration = approhimateDuration / turnsTotal + UnityEngine.Random.Range (-0.3f, 0.5f);
			float angle = UnityEngine.Random.Range (120f, 180f);
			var tickData = data.getTickData ();
			if (tickData == null) {
				yield break;
			}
			var newDir = Math2d.RotateVertexDeg (tickData.dirNorm, tickData.evadeSign * angle);
			SetFlyDir (newDir);

			var wait = WaitForSeconds(duration);
			while (wait.MoveNext()) yield return true;
		}
	}

}

//public class InivisibleShipBeh : CommonBeh{
//	IBehaviour current = null;
//	MInvisibleSpaceshipData.InvisibleData invisData; 
//	bool invisibleBehaviour = true;
//	InivisibleShipBeh (Data data, MInvisibleSpaceshipData.InvisibleData invisData) : base (data) {
//		this.invisData = invisData;
//	}
//
//	public override void Tick (float delta)	{
//		base.Tick (delta);
//		current.Tick (delta);
//	}
//
//	private IEnumerator BehaviourChangeLogic()
//	{
//		while (true) {
//			invisibleBehaviour = true;
//			thisShip.invisibilityComponent.SetState (true);
//			yield return new WaitForSeconds (invisData.fadeOutDuration + invisData.invisibleDuration);
//			thisShip.invisibilityComponent.SetState (false);
//
//			yield return new WaitForSeconds (invisData.fadeInDuration);
//
//			invisibleBehaviour = false;
//			yield return new WaitForSeconds (invisData.attackDutation);
//			//coward
//		}
//	}
//
//
//}
//

