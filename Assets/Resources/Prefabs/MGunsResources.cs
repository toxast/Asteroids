using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MGunsResources : ResourceSingleton<MGunsResources> 
{
	[SerializeField] public List<MGunData> guns;
	[SerializeField] public List<MRocketGunData> rocketLaunchers;
	[SerializeField] public List<MSpawnerGunData> spawnerGuns;
	[SerializeField] public List<MLazerGunData> lazerGuns;
}
