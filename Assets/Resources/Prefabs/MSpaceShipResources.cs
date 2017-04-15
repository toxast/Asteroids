using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MSpaceShipResources : ResourceSingleton<MSpaceShipResources> 
{
	[SerializeField] public List<ShipUpgrades> userSpaceships;
	[SerializeField] bool testid = false;

	void OnValidate(){
		CheckIds ();
	}

	public void CheckIds(){
		testid = false;
		List<MSpaceshipData> allSpaceships = new List<MSpaceshipData> ();
		for (int i = 0; i < userSpaceships.Count; i++) {
			allSpaceships.AddRange (userSpaceships [i].ships);
		}

		for (int i = 0; i < allSpaceships.Count; i++) {
			var id = allSpaceships [i].id;
			if (id <= 0) {
				Debug.LogError ("wrong ship id " + allSpaceships[i].name);
			}
			for (int k = 0; k < allSpaceships.Count; k++) {
				if (k != i && allSpaceships [k].id == id) {
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

[System.Serializable]
public class ShipUpgrades
{
	public List<MSpaceshipData> ships;
}