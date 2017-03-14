using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class DOTEffect : TickableEffect 
{
	protected Data data;
	protected float timeLeft;
	protected float currentDps;

	public DOTEffect(Data data) {
		this.data = data;
		timeLeft = data.duration;
		currentDps = data.dps;
	}

	public override void Tick (float delta) {
		base.Tick (delta);
		if (!IsFinished ()) {
			timeLeft -= delta;
			ActionFunc(currentDps * delta);
		}
	}

	public virtual void ActionFunc(float amount){
		holder.Hit (amount);
	}

	public override bool IsFinished() {
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
			data = same.data;
		}

		float totalDamageToBeDone = timeLeft * currentDps + same.data.dps * same.data.duration;

		currentDps = maxDps;
		timeLeft = Mathf.Min(totalDamageToBeDone / maxDps, maxDuration);
	}

	[System.Serializable]
	public class Data{
		public float duration = 0;
		public float dps = 0;
		public float maxBuildUpDuration = 0;
		public ParticleSystemsData effect;
	}
}