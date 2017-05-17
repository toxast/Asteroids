using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public interface IBehaviour : ITickable {
	event Action<bool> OnAccelerateChange;
	event Action<bool> OnShootChange;
	event Action<Vector2> OnDirChange;
	event Action OnBrake;
	bool CanBeInterrupted ();
	bool IsUrgent();
	bool IsReadyToAct (); //can be called
	void Start(); //always call isReadyToAct before starting
	bool IsFinished(); //action finished itself
	void Stop(); //notify this action is stopped, and will not be ticked
	void PassiveTick (float delta); //used to tick delay, when this is not the current action
	bool PassiveTickOtherBehs();
}
