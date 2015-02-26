using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpaceshipsResources : ResourceSingleton<SpaceshipsResources> 
{
	[SerializeField] int clone;
	[SerializeField] public List<FullSpaceShipSetupData> spaceships;


	[ContextMenu ("clone spaceship")]
	void CloneRocketLauncher () 
	{
		spaceships.Add (spaceships [clone].Clone());
	}
}
