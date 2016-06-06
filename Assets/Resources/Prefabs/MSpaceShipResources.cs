using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MSpaceShipResources : ResourceSingleton<MSpaceShipResources> 
{
	[SerializeField] public List<MSpaceshipData> spaceships;
	[SerializeField] public List<MTurretData> turrets;
	[SerializeField] public List<MTowerData> towers;
}