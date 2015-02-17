using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class GunsResources : ResourceSingleton<GunsResources> 
{
	[SerializeField] int clone;

	[ContextMenu ("clone rocketLauncher")]
	void CloneRocketLauncher () 
	{
		rocketLaunchers.Add (rocketLaunchers [clone]);
	}

	[ContextMenu ("clone gun")]
	void CloneGun () 
	{
		guns.Add (guns [clone]);
	}


	[SerializeField] public List<GunData> guns;
	[SerializeField] public List<RocketLauncherData> rocketLaunchers;
}
