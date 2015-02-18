using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpaceshipsResources : ResourceSingleton<SpaceshipsResources> 
{
	[SerializeField] public List<FullSpaceShipSetupData> spaceships;
}
