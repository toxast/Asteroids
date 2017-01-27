using UnityEngine;
using System.Collections;

public interface ITickable
{
	void Tick (float delta);
}

public abstract class TickableEffect : ITickable 
{
	protected abstract eType etype{ get;}
	protected PolygonGameObject holder;

	public void SetHolder(PolygonGameObject holder) {
		this.holder = holder;
	}
	public virtual void Tick(float delta) { }

	public virtual bool IsFinished(){
		return false;
	}

	public virtual bool CanBeUpdatedWithSameEffect{get{ return false;}}
	public bool IsTheSameEffect(TickableEffect effect){ return effect.etype == this.etype; }
	public virtual void UpdateBy(TickableEffect sameEffect) { }

	protected enum eType {
		None = 0,
		GravityShield,
		Burning,
	}
}


public class DOTEffect : TickableEffect 
{
	protected override eType etype { get { return eType.Burning; } }
	Data data;
	float timeLeft;
	float currentDps;

	public DOTEffect(Data data) {
		this.data = data;
		timeLeft = data.duration;
		currentDps = data.dps;
	}

	public override void Tick (float delta) {
		base.Tick (delta);
		if (!IsFinished ()) {
			timeLeft -= delta;
			holder.Hit (currentDps * delta);
		}
	}

	public virtual bool IsFinished() {
		return timeLeft <= 0;
	}

	/// <summary>
	/// repalces current dps with the maximum dps, adjusts the duration so the total damage is preserved
	/// duration is cut by the effect with the max dps
	/// </summary>
	public override void UpdateBy (TickableEffect sameEffect) {
		base.UpdateBy (sameEffect);
		var same = sameEffect as DOTEffect;

		float maxDps;
		float maxDuration;
		if (currentDps > same.data.dps) {
			maxDps = currentDps;
			maxDuration = data.maxBuildUpDuration;
		} else {
			maxDps = same.data.dps;
			maxDuration = same.data.maxBuildUpDuration;
		}

		float totalDamageToBeDone = timeLeft * currentDps + same.data.dps * same.data.duration;

		currentDps = maxDps;
		timeLeft = Mathf.Min(totalDamageToBeDone / maxDps, maxDuration);
	}

	[System.Serializable]
	public class Data{
		public float duration = 1;
		public float dps = 3;
		public float maxBuildUpDuration = 5;
		//TODO: add effect particle system here
	}
}

//public class GravityShieldEffect : TickableEffect 
//{
//	override eType etype { get { return eType.GravityShield; } }
//	Data data;
//	float timeLeft;
//	float currentDps;
//
//	public DOTEffect(Data data) {
//		this.data = data;
//		timeLeft = data.duration;
//		currentDps = data.dps;
//	}
//
//	public override void Tick (float delta) {
//		base.Tick (delta);
//		timeLeft -= delta;
//	}
//
//	public virtual bool IsFinished() {
//		return timeLeft <= 0;
//	}
//
//	/// <summary>
//	/// repalces current dps with the maximum dps, adjusts the duration so the total damage is preserved
//	/// duration is cut by the effect with the max dps
//	/// </summary>
//	public override void UpdateBy (TickableEffect sameEffect) {
//		base.UpdateBy (sameEffect);
//		var same = sameEffect as DOTEffect;
//
//		float maxDps;
//		float maxDuration;
//		if (currentDps > same.data.dps) {
//			maxDps = currentDps;
//			maxDuration = data.maxBuildUpDuration;
//		} else {
//			maxDps = same.data.dps;
//			maxDuration = same.data.maxBuildUpDuration;
//		}
//
//		float totalDamageToBeDone = timeLeft * currentDps + same.data.dps * same.data.duration;
//
//		currentDps = maxDps;
//		timeLeft = Mathf.Min(totalDamageToBeDone / maxDps, maxDuration);
//	}
//
//	[System.Serializable]
//	public class Data{
//		public float duration = 1;
//		public float dps = 3;
//		public float maxBuildUpDuration = 5;
//		//TODO: add effect particle system here
//	}
//}