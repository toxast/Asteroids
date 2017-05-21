using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class SequentialBehs : BaseBeh {

	IBehaviour currentBeh;
	List<IBehaviour> behs;
	int current;

	public SequentialBehs( List<IBehaviour> behs){
		this.behs = new List<IBehaviour> (behs);
		current = 0;
		ChooseBeh(behs[0]);
	}

	void ChooseBeh(IBehaviour beh){
		if (currentBeh != null) {
			Unsubscribe (currentBeh);
		}
		currentBeh = beh;
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

	public override void Stop ()	{
		currentBeh.Stop ();
		current++;
		if (current >= behs.Count) {
			current = 0;
		}
		ChooseBeh (behs[current]);
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
