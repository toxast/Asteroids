using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public interface IDelayFlag : ITickable{
	bool passed{ get;} 
	void Set ();
	void SetOnMin ();

}
