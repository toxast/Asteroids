using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class UserContraller : CommonController {

	public UserContraller (SpaceShip thisShip, List<PolygonGameObject> bullets, Gun gun, AccuracyData accData) : base(thisShip, bullets, gun, accData)
	{
		thisShip.StartCoroutine (HackPullPowerups ());
	}

	IEnumerator HackPullPowerups(){
		var drops = Singleton<Main>.inst.pDrops;
		while (true) {
			var pup = drops.Find (d => d is PowerUp);
			if (pup != null) {
				pup.Accelerate (lastDelta, 3f, 1f, 8f, 8f*8f, (thisShip.position - pup.position).normalized);
			}
			yield return null;
		}
	}

	protected override IEnumerator ComfortTurn () {
		if (Math2d.Chance (0.5f)) {
			yield return base.ComfortTurn ();
		} else {
			float duration = new RandomFloat (2f, 3f).RandomValue;
			yield return AIHelper.TimerR(duration, LastDelta, Shoot);
		}
	}

	float LastDelta(){
		return lastDelta;
	}

	void Shoot() {
		accelerating = true;
		Shoot (accuracy, bulletsSpeed);
	}
}