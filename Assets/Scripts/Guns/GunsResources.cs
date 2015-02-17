using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class GunsResources : ResourceSingleton<GunsResources> 
{
	[SerializeField] public GunData[] guns;
}
