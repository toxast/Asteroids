using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public abstract class BaseBeh : IBehaviour {
	public class Data {
		public SpaceShip thisShip;
		public AIHelper.AccuracyChangerAdvanced accuracyChanger;
		public Func<AIHelper.Data> getTickData;
		public float comformDistanceMin;
		public float comformDistanceMax;
		public Gun mainGun;
	}

	public event Action<bool> OnAccelerateChange;
	public event Action<bool> OnShootChange;
	public event Action<Vector2> OnDirChange;
	public event Action OnBrake;

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

	public virtual bool IsUrgent () {return false;}
	public virtual bool CanBeInterrupted () {return false;}
	public virtual bool PassiveTickOtherBehs(){return false;}

	protected Data data;
	protected SpaceShip thisShip{get{return data.thisShip;}}
	protected PolygonGameObject target{ get{ return thisShip.target; }}

	public BaseBeh (Data data){
		this.data = data; 
	}

	public virtual void PassiveTick (float delta) { }
	public abstract bool IsReadyToAct ();
	public abstract bool IsFinished ();
	public virtual void Start () {
        Debug.LogWarning(this.GetType());
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
}
