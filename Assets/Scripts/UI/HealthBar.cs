using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HealthBar : ProgressBar
{
	protected void Awake()
	{
		//base.Awake ();
		GameResources.healthChanged += Display;
	}
}
