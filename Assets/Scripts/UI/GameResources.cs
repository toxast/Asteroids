using UnityEngine;
using System;
using System.Collections;

public static class GameResources 
{
	static public event Action<float> healthChanged;
	static private float shipHealth;
	static public void SetHealth(float h)
	{
		shipHealth = h;

		if(healthChanged != null)
			healthChanged (h);
	}

}
