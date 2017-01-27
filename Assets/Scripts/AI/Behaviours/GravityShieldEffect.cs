using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GravityShieldEffect : TickableEffect 
{
	protected override eType etype { get { return eType.GravityShield; } }
	Data data;
	float timeLeft;
	float currentForce;
	List<PolygonGameObject> gobjects;
	List<PolygonGameObject> bullets;

	public GravityShieldEffect(Data data) {
		this.data = data;
		timeLeft = data.duration;
		currentForce = data.force;
		gobjects = Singleton<Main>.inst.gObjects;
		bullets = Singleton<Main>.inst.bullets;
	}

	public override void Tick (float delta) {
		base.Tick (delta);

		if (!IsFinished ()) {
			timeLeft -= delta;
			new GravityForceExplosion (holder.position, data.range, 0, delta * currentForce, gobjects, holder.collision);
			new GravityForceExplosion (holder.position, data.range, 0, delta * currentForce, bullets, holder.collision);
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
		var same = sameEffect as GravityShieldEffect;
		timeLeft += same.data.duration;
		currentForce = Mathf.Max (currentForce, same.data.force);
	}

	[System.Serializable]
	public class Data{
		public float duration = 30;
		public float range = 20;
		public float force = 20;
		//TODO: add effect particle system here
	}
}