using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
		HeavyBullet,
		ExtraGuns,
		RotatingObjectsShield,
		KeepRotation,
		HealOT,
		HealOnce,
	}
}
