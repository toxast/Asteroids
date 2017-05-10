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

	List<float> dmgs = new List<float> ();
	float waveDmgDealt = 0;
	public RepeatWave(MRepeatWave data) {
		this.data = data;
		countLeft = data.count;
	}

	public bool Done () {
		return (current == null || current.Done()) && countLeft <= 0;
	}

	void Log(string str){
		Logger.Log (str);
		Debug.LogError (str);
	}

	public void Tick ()
	{
		if ((current == null || current.Done ())) {
			if (current != null) {
				Debug.Log ((data.count - countLeft) + " wave finished, dmg dealt: " + waveDmgDealt);
				dmgs.Add (waveDmgDealt);
				if (countLeft == 0) {
					OnWavesFinished ();
				}
			} else {
				if (data.wave is MFixedWave) {
					var fwave = data.wave as MFixedWave;
					var fobj = fwave.waveData.objects [0];
					Log(string.Format ("starting {0} waves of {1} count: {2}", data.count, fobj.spawn.name, fobj.count));
				} else {
					Log(string.Format ("starting {0} waves of {1}", data.count, data.wave.name));
				}
			}

			if (countLeft > 0) {
				user.RestoreShield ();
				current = data.wave.GetWave ();
				waveDmgDealt = 0;
				countLeft--;
			}
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

	void OnWavesFinished(){
		if (data.wave is MFixedWave) {
			var fwave = data.wave as MFixedWave;
			dmgs.Sort ();
			var dmgstr = MyExtensions.FormString (dmgs);
			int mesures = 0;
			int min = Mathf.RoundToInt (dmgs.Count * 0.4f);
			int max = Mathf.RoundToInt (dmgs.Count * 0.7f);
			float total = 0;
			for (int i = 0; i < dmgs.Count; i++) {
				if (i >= min && i <= max) {
					mesures++;
					total += dmgs [i];
				}
			}

			float middleforWave = total / mesures;
			var fobj = fwave.waveData.objects [0];
			float middleforElem = middleforWave / fobj.count;
			Log ("dmgs: " + dmgstr);
			Log ("middleforWave:  " + middleforWave + " = " + total + "/" + mesures);
			Log ("middleforElem:  " + middleforElem + " = " + middleforWave + "/" + fobj.count);
			Log (string.Format ("finished waves of {0} dmg: {1} difficulty: {2}", fobj.spawn.name, middleforElem, Mathf.RoundToInt(middleforElem * 5f)));
		}
	}
}
