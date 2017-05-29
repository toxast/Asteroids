using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPowerups : MonoBehaviour 
{
	[SerializeField] ProgressBar powerupPrefab;
	[SerializeField] Transform container;

	List<UserComboPowerup.ProgressColorWrapper> currentPowerups = new List<UserComboPowerup.ProgressColorWrapper>();
	List<ProgressBar> currentBars = new List<ProgressBar>();

	public void DisplayPowerup(UserComboPowerup.ProgressColorWrapper powerup) {
		if (powerup == null) {
			return;
		}
		if (currentPowerups.Exists (e => e.progressObj == powerup.progressObj)) {
			Debug.LogError ("same effect " + powerup.progressObj.GetType().ToString());
			return;
		}
		var bar = Instantiate (powerupPrefab, container);
		bar.SetBarColor (powerup.color);
		currentPowerups.Add (powerup);
		currentBars.Add (bar);
	}

	public void Clear() {
		for (int i = currentPowerups.Count - 1; i >= 0; i--) {
			DestroyAt (i);
		}
	}

	void Update(){
		for (int i = currentPowerups.Count - 1; i >= 0; i--) {
			var progress = currentPowerups [i].iprogress;
			if (progress >= 1) {
				DestroyAt (i);
			} else {
				currentBars [i].Display (1f - progress);
			}
		}
	}

	void DestroyAt(int i){
		Destroy (currentBars [i].gameObject);
		currentPowerups.RemoveAt (i);
		currentBars.RemoveAt (i);
	}
}
