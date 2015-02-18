using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class GunsResources : ResourceSingleton<GunsResources> 
{
	[SerializeField] int clone;
	[SerializeField] public List<GunData> guns;
	[SerializeField] public List<RocketLauncherData> rocketLaunchers;


	[ContextMenu ("clone rocketLauncher")]
	void CloneRocketLauncher () 
	{
		rocketLaunchers.Add (rocketLaunchers [clone].Clone());
	}

	[ContextMenu ("clone gun")]
	void CloneGun () 
	{
		guns.Add (guns [clone].Clone());
	}



}
