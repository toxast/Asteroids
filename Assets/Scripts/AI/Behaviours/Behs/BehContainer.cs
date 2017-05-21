using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public abstract class BehContainer : IBehaviour{
	public abstract bool CanBeInterrupted ();
	public abstract bool IsUrgent ();
	public abstract bool IsReadyToAct ();
	public abstract void Start ();
	public abstract bool IsFinished ();
	public abstract void Stop ();
	public abstract void PassiveTick (float delta);
	public abstract bool PassiveTickOtherBehs ();
	public abstract void Tick (float delta);

	public event Action<bool> OnAccelerateChange;
	public event Action<bool> OnShootChange;
	public event Action<Vector2> OnDirChange;
	public event Action OnBrake;

	protected void HandleAccelerateChange(bool acc){
		OnAccelerateChange (acc);
	}

	protected void HandleShootChange(bool shoot){
		OnShootChange(shoot);
	}

	protected void HandleDirChange(Vector2 dir){
		OnDirChange(dir);
	}

	protected void HandleBrake(){
		OnBrake ();
	}

	protected void Subscribe(IBehaviour beh){
		if (beh != null) {
			beh.OnAccelerateChange += HandleAccelerateChange;
			beh.OnDirChange += HandleDirChange;
			beh.OnShootChange += HandleShootChange;
			beh.OnBrake += HandleBrake;
		}
	}

	protected void Unsubscribe(IBehaviour beh){
		if (beh != null) {
			beh.OnAccelerateChange -= HandleAccelerateChange;
			beh.OnDirChange -= HandleDirChange;
			beh.OnShootChange -= HandleShootChange;
			beh.OnBrake -= HandleBrake;
		}
	}
}
