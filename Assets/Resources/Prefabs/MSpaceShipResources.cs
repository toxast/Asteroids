using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MSpaceShipResources : ResourceSingleton<MSpaceShipResources> 
{
	[SerializeField] public List<MSpaceshipData> userSpaceships;
	[SerializeField] bool testid = false;

	void OnValidate(){
		CheckIds ();
	}

	private void CheckIds(){
		for (int i = 0; i < userSpaceships.Count; i++) {
			var id = userSpaceships [i].id;
			if (id <= 0) {
				Debug.LogError ("wrong ship id " + userSpaceships[i].name);
			}
			for (int k = 0; k < userSpaceships.Count; k++) {
				if (k != i && userSpaceships [k].id == id) {
					Debug.LogError (i + " " + k + " user ships has same id " + id);				
				}
			}
		}
	}

	/*[ContextMenu ("calculate health")] 
	private void CalculateHealth()
	{
		for (int i = 0; i < spaceships.Count; i++) {
			if (spaceships [i] != null) 
			{
				float area;
				Math2d.GetMassCenter(spaceships [i].verts, out area);
				spaceships [i].physical.health = Mathf.Pow (area, 0.8f) * spaceships [i].physical.healthModifier / 2f;
			}
		}
	}*/
}