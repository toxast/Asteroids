using UnityEngine;
using System.Collections;

public class ShieldsBar : ProgressBar
{
	protected void Awake()
	{
		//base.Awake ();
		GameResources.shieldsChanged += Display;
	}
}
