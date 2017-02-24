using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicalChangesEffect : TickableEffect 
{
	protected override eType etype { get { return eType.PhysicalChanges; } }
	public override bool CanBeUpdatedWithSameEffect { get { return false; } }

	protected Data data;
	protected float timeLeft;

	float lastDefence = -1;

	public PhysicalChangesEffect(Data data) {
		this.data = data;
		timeLeft = data.duration;
	}

	public override void Tick (float delta) {
		base.Tick (delta);
		if (!IsFinished ()) {
			timeLeft -= delta;
			if (IsFinished ()) {
				ResumeHolderValues ();
			}
		}
	}

	public override bool IsFinished() {
		return timeLeft <= 0;
	}

	public override void SetHolder (PolygonGameObject holder) {
		base.SetHolder (holder);
		holder.MultiplyMass (data.multiplyMass);
		holder.MultiplyCollisionAttack (data.multiplyCollisionAttack);
		if (data.overrideDefence != -1) { //yes defence can be restored wrong if few effect
			lastDefence = holder.collisionDefence;
			holder.ChangeCollisionDefence (data.overrideDefence);
		}
		var spaceshipHolder = holder as SpaceShip;
		if (spaceshipHolder != null) {
			spaceshipHolder.MultiplyMaxSpeed (data.multiplyMaxSpeed);
			spaceshipHolder.MultiplyStability(data.multiplyStability);
			spaceshipHolder.MultiplyThrust (data.multiplyThrust);
			spaceshipHolder.MultiplyTurnSpeed(data.multiplyTurnSpeed);
		}
	}

	void ResumeHolderValues(){
		holder.MultiplyMass (1f/data.multiplyMass);
		holder.MultiplyCollisionAttack (1f/data.multiplyCollisionAttack);
		if (data.overrideDefence != -1) { //yes defence can be restored wrong if few effect
			holder.ChangeCollisionDefence (lastDefence);
		}
		var spaceshipHolder = holder as SpaceShip;
		if (spaceshipHolder != null) {
			spaceshipHolder.MultiplyMaxSpeed (1f/data.multiplyMaxSpeed);
			spaceshipHolder.MultiplyStability(1f/data.multiplyStability);
			spaceshipHolder.MultiplyThrust (1f/data.multiplyThrust);
			spaceshipHolder.MultiplyTurnSpeed(1f/data.multiplyTurnSpeed);
		}
	}

	public override void HandleHolderDestroying () {
		base.HandleHolderDestroying ();
		ResumeHolderValues ();
	}

	[System.Serializable]
	public class Data {
		public float duration = 10f;
		public float overrideDefence = -1;
		public float multiplyMass = 1;
		public float multiplyCollisionAttack = 1;
		public float multiplyThrust = 1f;
		public float multiplyMaxSpeed = 1f;
		public float multiplyTurnSpeed = 1f;
		public float multiplyStability = 1f;
	}
}