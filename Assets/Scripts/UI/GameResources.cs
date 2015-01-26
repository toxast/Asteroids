using UnityEngine;
using System;
using System.Collections;

public static class GameResources 
{
	static public event Action<float> healthChanged;
	static public void SetHealth(float h)
	{
		if(healthChanged != null)
			healthChanged (h);
	}


	static public event Action<float> shieldsChanged;
	static public void SetShields(float s)
	{
		if(shieldsChanged != null) 
			shieldsChanged (s);
	}

}
