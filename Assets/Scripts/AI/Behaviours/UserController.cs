using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class UserController : CommonController {

	public UserController (SpaceShip thisShip, List<PolygonGameObject> bullets, Gun gun, AccuracyData accData) : base(thisShip, bullets, gun, accData)
	{
		thisShip.StartCoroutine (HackPullPowerups ());
	}

	public override bool shooting {
		get {
			return true;
		}
		protected set {
			base.shooting = value;
		}
	}

	IEnumerator HackPullPowerups(){
		var drops = Singleton<Main>.inst.pDrops;
		while (true) {
			var pup = drops.Find (d => d is PowerUp);
			if (pup != null) {
				pup.Accelerate (Time.deltaTime, 3f, 1f, 12f, 12f*12f, (thisShip.position - pup.position).normalized);
			}
			yield return null;
		}
	}

	protected override IBehaviour GetTurnBeh (CommonBeh.Data behData) {
		return new UserTurnBeh (behData, new NoDelayFlag());
	}

//	protected override IEnumerator NoTargetBeh (float duration)	{
//		turnDirection = thisShip.cacheTransform.right;
//		turnDirection = Math2d.MakeRight (turnDirection);
//		SetAcceleration (true);
//		yield return new WaitForSeconds (duration); 
//	}

}