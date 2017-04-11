using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MPowerUpResources : ResourceSingleton<MPowerUpResources> {
	[SerializeField] public List<GalleryPowerUpData> powerups;

	[SerializeField] bool testid = false;

	void OnValidate(){
		CheckIds ();
	}

	private void CheckIds(){
		//Debug.LogError ("TODO: powerup ids");

		testid = false;
		List<MCometData> allitems= new List<MCometData> ();
		for (int i = 0; i < powerups.Count; i++) {
			allitems.AddRange (powerups [i].comets);
		}

		for (int i = 0; i < allitems.Count; i++) {
			var id = allitems [i].id;
			if (id <= 0) {
				Debug.LogError ("wrong comet id " + allitems[i].name);
			}
			for (int k = 0; k < allitems.Count; k++) {
				if (k != i && allitems [k].id == id) {
					Debug.LogError (i + " " + k + " user powerups has same id " + id);				
				}
			}
		}
	}
}
