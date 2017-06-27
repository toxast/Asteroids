using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthBoss : PolygonGameObject 
{
	MEarthBossData data;
	AdvancedTurnComponent turnComponent;

	public void Init(MEarthBossData data){
		this.data = data;

		turnComponent = new AdvancedTurnComponent (this, data.rotationSpeed);

		var shoulder = data.shoulder.Create ();
		shoulder.position = this.position;
		shoulder.AddEffect(new KeepOrientationEffect(data.shoulderOrientationData, this));
		shoulder.AddEffect(new KeepPositionEffect(data.shoulderPositionData, this));
		Singleton<Main>.inst.Add2Objects (shoulder);
	}

	protected override void ApplyRotation (float dtime) {
		if (TargetNotNull) {
			turnComponent.TurnByDirection (target.position - this.position, dtime);
		} else {
			turnComponent.TurnByDirection (cacheTransform.right, dtime);
		}
	}

	public override void Tick (float delta)	{
		base.Tick (delta);
	}
}