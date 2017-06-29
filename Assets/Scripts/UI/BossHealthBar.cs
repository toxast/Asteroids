using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHealthBar : MonoBehaviour {

	[SerializeField] ProgressBar bar;

	List<PolygonGameObject> bossObjects = new List<PolygonGameObject> ();
	public void Add(PolygonGameObject bossObj){
		bossObjects.Add (bossObj);
		UpdateHealthBar ();
	}

	public void Clear(){
		bossObjects.Clear ();
		UpdateHealthBar ();
	}

	void Update(){
		UpdateHealthBar ();
	}

	void UpdateHealthBar(){
		bar.gameObject.SetActive (bossObjects.Count > 0);
		if (bossObjects.Count > 0) {
			//bossObjects.RemoveAll (b => Main.IsNull (b));
			float total = 0;
			float left = 0;
			foreach (var item in bossObjects) {
				if (!Main.IsNull (item)) {
					total += item.fullHealth;
					left += item.GetLeftHealthPersentage () * item.fullHealth;
				}
			}
			if (total > 0) {
				bar.Display (left / total);
			} else {
				bar.Display(0);
			}
		}
	}
}
