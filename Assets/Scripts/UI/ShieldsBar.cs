using UnityEngine;
using System.Collections;

public class ShieldsBar : ProgressBar
{
	protected override void Awake()
	{
		base.Awake ();
		GameResources.shieldsChanged += Display;
	}
}
