using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class LevelsResources : ResourceSingleton<LevelsResources> {
	[SerializeField] public List<SpawnWaves> levels;

    public bool runUpdate = false;
    void Update()
    {
        if (runUpdate) {
            runUpdate = false;
        }
    }
}
