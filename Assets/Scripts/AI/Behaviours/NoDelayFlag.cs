using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class NoDelayFlag : IDelayFlag {
	public bool passed{ get{ return true;} } 
	public void Set () {	}
	public void SetOnMin () {	}
	public void Tick (float delta)	{	}
}
