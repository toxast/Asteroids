using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public abstract class BaseBeh : IBehaviour {
	public event Action<bool> OnAccelerateChange;
	public event Action<bool> OnShootChange;
	public event Action<Vector2> OnDirChange;
	public event Action OnBrake;

//	protected Func<bool> forceFinisher = null;
//	public void SetForceFinisher(Func<bool> stop){
//		forceFinisher = stop;
//	}
//	protected bool DoForceStop(){
//		return forceFinisher != null && forceFinisher ();
//	}

	public void FireAccelerateChange(bool acc){
		if (OnAccelerateChange != null) {
			OnAccelerateChange (acc);
		}
	}
	public void FireShootChange(bool shoot){
		if (OnShootChange != null) {
			OnShootChange (shoot);
		}
	}
	public void FireDirChange(Vector2 dir){
		if (OnDirChange != null) {
			OnDirChange (dir);
		}
	}
	public void FireBrake(){
		if (OnBrake != null) {
			OnBrake ();
		}
	}

	protected bool _isUrgent = false;
	public virtual bool IsUrgent () {return _isUrgent;}

	protected bool _canBeInterrupted = false;
	public virtual bool CanBeInterrupted () {return _canBeInterrupted;}

	protected bool _passiveTickOthers = false;
	public virtual bool PassiveTickOtherBehs(){return _passiveTickOthers;}
	public void SetPassiveTickOthers(bool tick){
		_passiveTickOthers = tick;
	}

	public virtual void PassiveTick (float delta) { }
	public abstract bool IsReadyToAct ();
	public abstract bool IsFinished ();
	public virtual void Start () {
        //Debug.LogWarning(this.GetType());
    }
	public virtual void Stop () {
        //Debug.LogWarning("Stop " + this.GetType());
    }

	float lastDelta = 0;
	public virtual void Tick (float delta) {
		lastDelta = delta;
	}
	protected float DeltaTime() {
		return lastDelta;
	}

	protected void SetFlyDir(Vector2 dir, bool accelerating = true, bool shooting = false){
		OnDirChange(dir);
		OnAccelerateChange (accelerating);
		OnShootChange (shooting);
	}

	protected IEnumerator WaitForSeconds(float duration){
		return AIHelper.TimerR (duration, DeltaTime);
	}

	protected void Subscribe(IBehaviour beh){
		if (beh != null) {
			beh.OnAccelerateChange += FireAccelerateChange;
			beh.OnDirChange += FireDirChange;
			beh.OnShootChange += FireShootChange;
			beh.OnBrake += FireBrake;
		}
	}

	protected void Unsubscribe(IBehaviour beh){
		if (beh != null) {
			beh.OnAccelerateChange -= FireAccelerateChange;
			beh.OnDirChange -= FireDirChange;
			beh.OnShootChange -= FireShootChange;
			beh.OnBrake -= FireBrake;
		}
	}

}
