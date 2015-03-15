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


	[ContextMenu ("sort")]
	void SortIt () 
	{
		spaceships.Sort ((a,b) => 
				{
					float aArea;
					Math2d.GetMassCenter (a.verts, out aArea);
					float bArea;
					Math2d.GetMassCenter (b.verts, out bArea);
					return aArea.CompareTo(bArea);
				});
	}
}
