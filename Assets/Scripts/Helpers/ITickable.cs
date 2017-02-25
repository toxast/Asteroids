using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface ITickable
{
	void Tick (float delta);
}

public abstract class TickableEffect : ITickable 
{
	protected abstract eType etype{ get;}
	protected PolygonGameObject holder;

	public virtual void SetHolder(PolygonGameObject holder) {
		this.holder = holder;
	}
	public virtual void Tick(float delta) { }

	public virtual bool IsFinished(){
		return false;
	}

	public virtual bool CanBeUpdatedWithSameEffect{get{ return false;}}
	public bool IsTheSameEffect(TickableEffect effect){ return effect.etype == this.etype; }
	public virtual void UpdateBy(TickableEffect sameEffect) { }
	public virtual void HandleHolderDestroying(){ }

	protected enum eType {
		None = 0,
		GravityShield,
		Burning,
        GunsShow,
		PhysicalChanges,
		SpawnBackup,
		HeavvyBullet,
	}
}


public class HeavvyBulletEffect : TickableEffect 
{
	protected override eType etype { get { return eType.HeavvyBullet; } }
	public override bool CanBeUpdatedWithSameEffect { get { return true; } }

	Data data;
	float timeLeft;

	public HeavvyBulletEffect(Data data) {
		this.data = data;
		timeLeft = data.duration;
	}

	public override void SetHolder (PolygonGameObject holder) {
		base.SetHolder (holder);
		holder.heavyBulletData = data;
	}

	public override bool IsFinished() {
		return timeLeft <= 0;
	}

	public override void Tick (float delta) {
		base.Tick (delta);
		if (!IsFinished ()) {
			timeLeft -= delta;
			if (IsFinished ()) {
				holder.heavyBulletData = null;
			}
		}
	}

	public override void UpdateBy (TickableEffect sameEffect) {
		base.UpdateBy (sameEffect);
		var same = sameEffect as HeavvyBulletEffect;
		timeLeft += same.data.duration;
		data = same.data;
		holder.heavyBulletData = same.data;
	}

	[System.Serializable]
	public class Data{
		public float duration;
		public float multiplier;
	}
}




