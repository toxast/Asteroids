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
		HeavyBullet,
		ExtraGuns,
		RotatingObjectsShield,
		KeepRotation,
	}
}

public class ExtraGunsEffect : TickableEffect {
	protected override eType etype { get { return eType.ExtraGuns; } }
	public override bool CanBeUpdatedWithSameEffect { get { return false; } }

	Data data;
	float timeLeft;
	List<Gun> guns;

	public ExtraGunsEffect(Data data) {
		this.data = data;
		timeLeft = data.duration;
	}

	public override void SetHolder (PolygonGameObject holder) {
		base.SetHolder (holder);
		guns = new List<Gun> ();
		foreach (var gunplace in data.guns) {
			var gun = gunplace.GetGun (holder);
			guns.Add (gun);
		}
		holder.AddExtraGuns (guns);
	}

	public override bool IsFinished() {
		return timeLeft <= 0;
	}

	public override void Tick (float delta) {
		base.Tick (delta);
		if (!IsFinished ()) {
			timeLeft -= delta;
			if (IsFinished ()) {
				holder.RemoveGuns (guns);
			}
		}
	}

	[System.Serializable]
	public class Data{
		public float duration;
		public List<MGunSetupData> guns;
	}
}





