using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MSpaceShipResources : ResourceSingleton<MSpaceShipResources> 
{
	[SerializeField] public List<MSpaceshipData> spaceships;
	[SerializeField] public List<MTurretData> turrets;
	[SerializeField] public List<MTowerData> towers;

	[ContextMenu ("calculate health")] 
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
	}
}