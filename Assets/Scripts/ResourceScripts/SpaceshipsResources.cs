using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpaceshipsResources : ResourceSingleton<SpaceshipsResources> 
{
	[SerializeField] int clone;
	[SerializeField] public List<FullSpaceShipSetupData> spaceships;

	[SerializeField] public List<TurretSetupData> turrets;

	[SerializeField] public List<TowerSetupData> towers;

	[ContextMenu ("clone spaceship")]
	void CloneRocketLauncher () 
	{
		spaceships.Insert (clone + 1, spaceships [clone].Clone());
	}

//	[ContextMenu ("guns+1")]
//	void GunsIncrease () 
//	{
//		spaceships.ForEach (s => s.guns.ForEach (g => g.type =  (GunSetupData.eGuns)((int)(g.type) + 1)));
//	}


//	[ContextMenu ("sort")]
//	void SortIt () 
//	{
//		spaceships.Sort ((a,b) => 
//				{
//					float aArea;
//					Math2d.GetMassCenter (a.verts, out aArea);
//					float bArea;
//					Math2d.GetMassCenter (b.verts, out bArea);
//					return aArea.CompareTo(bArea);
//				});
//	}
}
