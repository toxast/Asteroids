using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class WeightedBehs : IBehaviour {
	public event Action<bool> OnAccelerateChange;
	public event Action<bool> OnShootChange;
	public event Action<Vector2> OnDirChange;
	public event Action OnBrake;

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
			currentBeh.OnAccelerateChange -= HandleAccelerateChange;
			currentBeh.OnDirChange -= HandleDirChange;
			currentBeh.OnShootChange -= HandleShootChange;
			currentBeh.OnBrake -= HandleBrake;
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

		currentBeh.OnAccelerateChange += HandleAccelerateChange;
		currentBeh.OnDirChange += HandleDirChange;
		currentBeh.OnShootChange += HandleShootChange;
		currentBeh.OnBrake += HandleBrake;
	}

	void HandleAccelerateChange(bool acc){
		OnAccelerateChange (acc);
	}

	void HandleShootChange(bool shoot){
		OnShootChange(shoot);
	}

	void HandleDirChange(Vector2 dir){
		OnDirChange(dir);
	}

	void HandleBrake(){
		OnBrake ();
	}

	public bool CanBeInterrupted ()  { return currentBeh.CanBeInterrupted(); }
	public bool IsUrgent () { return currentBeh.IsUrgent(); }
	public bool IsReadyToAct () { return currentBeh.IsReadyToAct(); }
	public bool IsFinished () { return currentBeh.IsFinished(); }
	public bool PassiveTickOtherBehs() {return currentBeh.PassiveTickOtherBehs();}

	public void Start ()
	{
		currentBeh.Start ();
	}

	public void Stop ()
	{
		currentBeh.Stop ();
		ChooseBeh ();
	}

	public void PassiveTick (float delta)
	{
		currentBeh.PassiveTick (delta);
	}

	public void Tick (float delta)
	{
		currentBeh.Tick (delta);
	}
}
