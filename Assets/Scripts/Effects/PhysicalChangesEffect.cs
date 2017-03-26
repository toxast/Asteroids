using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PhysicalChangesEffect : DurationEffect 
{
	protected override eType etype { get { return eType.PhysicalChanges; } }
	public override bool CanBeUpdatedWithSameEffect { get { return false; } }

	protected Data data;

	public PhysicalChangesEffect(Data data) : base(data) {
		this.data = data;
	}
   
	public override void SetHolder (PolygonGameObject holder) {
		base.SetHolder (holder);
		holder.MultiplyMass (data.multiplyMass);
		holder.MultiplyCollisionAttack (data.multiplyCollisionAttack);
		if (data.overrideDefence != -1) {
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
		if (data.overrideDefence != -1) { 
			holder.RestoreCollisionDefence ();
		}
		var spaceshipHolder = holder as SpaceShip;
		if (spaceshipHolder != null) {
			spaceshipHolder.MultiplyMaxSpeed (1f/data.multiplyMaxSpeed);
			spaceshipHolder.MultiplyStability(1f/data.multiplyStability);
			spaceshipHolder.MultiplyThrust (1f/data.multiplyThrust);
			spaceshipHolder.MultiplyTurnSpeed(1f/data.multiplyTurnSpeed);
		}
	}

    public override void OnExpired() {
        ResumeHolderValues();
    }

    public override void HandleHolderDestroying () {
		base.HandleHolderDestroying ();
		ResumeHolderValues ();
	}

	[System.Serializable]
	public class Data : IHasDuration, IApplyable {
		public float duration = 4;
		public float iduration{get {return duration;} set{duration = value;}}
		public float overrideDefence = -1;
		public float multiplyMass = 1;
		public float multiplyCollisionAttack = 1;
		public float multiplyThrust = 1f;
		public float multiplyMaxSpeed = 1f;
		public float multiplyTurnSpeed = 1f;
		public float multiplyStability = 1f;
		public IHasProgress Apply(PolygonGameObject picker) {
			var effect = new PhysicalChangesEffect (this);
			picker.AddEffect (effect);
			return effect;
		}
	}
}