using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelsResources : ResourceSingleton<LevelsResources> {
	[SerializeField] public List<SpawnWaves> levels;

//    [ContextMenu ("ReplaceDataWithPrefabs")]
//    private void ReplaceDataWithPrefabs (){
//        foreach (var lvl in levels) {
//            foreach (var wave in lvl.list) {
//                foreach (var obj in wave.objects) {
//                    obj.PlacePrefab ();
//                }
//            }
//        }
//    }
}
