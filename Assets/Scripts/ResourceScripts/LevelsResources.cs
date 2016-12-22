using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelsResources : ResourceSingleton<LevelsResources> {
	[SerializeField] public List<SpawnWaves> levels;

    [ContextMenu ("Check prefabs consistency")]
    private void ReplaceDataWithPrefabs (){
        bool errors = false;
        foreach (var lvl in levels) {
            foreach (var wave in lvl.list) {
                foreach (var obj in wave.objects) {
                    if (obj.prefab == null || !(obj.prefab is ISwanable)) {
                        Debug.LogError ("LevelsResources: prefab " + lvl.name + " " + wave.name + " " + obj.name + " is bad");
                        errors = true;
                    }
                }
            }
        }
        if (!errors) {
            Debug.Log("LevelsResources: everything is cool");
        }
    }
}
