using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using Rnd = UnityEngine.Random;

//heals user and logs the damage, use for editor only
public class RepeatWave : IWaveSpawner {
	MRepeatWave data;
	IWaveSpawner current;
	int countLeft;

	UserSpaceShip user {
		get{ return Singleton<Main>.inst.userSpaceship; }
	}

	float waveDmgDealt = 0;
	public RepeatWave(MRepeatWave data) {
		this.data = data;
		countLeft = data.count;
	}

	public bool Done () {
		return (current == null || current.Done()) && countLeft <= 0;
	}

	public void Tick ()
	{
		if ((current == null || current.Done ()) && countLeft > 0) {
			if (current != null) {
				Logger.Log ((data.count - countLeft) + " wave finished, dmg dealt: " + waveDmgDealt);
			} else {
				Logger.Log( string.Format("starting {0} waves of {1}", data.count, data.wave.name));
			}
			current = data.wave.GetWave ();
			waveDmgDealt = 0;
			countLeft--;
		}
		if (current != null) {
			current.Tick ();
			if (user != null) {
				float dmg = user.fullHealth - user.CurrentHealth ();
				if (dmg > 0.01f) {
					waveDmgDealt += dmg;
					user.Heal (dmg);
				}
			}
		}
	}
}
