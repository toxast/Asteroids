using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class WeightedBehs : BaseBeh {
	IBehaviour currentBeh;
	List<IBehaviour> behs;
	List<float> weights;
	float resetWeight;
	float addOthersWeight;

	public WeightedBehs(List<IBehaviour> behs, List<float> startWeights, float resetWeight, float addOthersWeight){
		if (behs.Count != startWeights.Count) {
			Debug.LogError("wrong behs 2 w8s count");
		}
		this.behs = new List<IBehaviour> (behs);
		this.weights = new List<float> (startWeights);
		this.resetWeight = resetWeight;
		this.addOthersWeight = addOthersWeight;
        ChooseBeh();
    }

	void ChooseBeh(){
		if (currentBeh != null) {
			Unsubscribe (currentBeh);
		}
		var indx = Math2d.Roll (weights);
		for (int i = 0; i < weights.Count; i++) {
			if (i == indx) {
				weights[i] = resetWeight;
			} else {
				weights[i] += addOthersWeight;
			}
		}
		currentBeh = behs[indx];
		Subscribe (currentBeh);
	}

	public override bool CanBeInterrupted ()  { return currentBeh.CanBeInterrupted(); }
	public override bool IsUrgent () { return currentBeh.IsUrgent(); }
	public override bool IsReadyToAct () { return currentBeh.IsReadyToAct(); }
	public override bool IsFinished () { return currentBeh.IsFinished(); }
	public override bool PassiveTickOtherBehs() {return currentBeh.PassiveTickOtherBehs();}

	public override void Start ()
	{
		currentBeh.Start ();
	}

	public override void Stop ()
	{
		currentBeh.Stop ();
		ChooseBeh ();
	}

	public override void PassiveTick (float delta)
	{
		currentBeh.PassiveTick (delta);
	}

	public override void Tick (float delta)
	{
		currentBeh.Tick (delta);
	}
}