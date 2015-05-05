using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelsResources : ResourceSingleton<LevelsResources> 
{
	[SerializeField] public List<SpawnWaves> levels;
}
