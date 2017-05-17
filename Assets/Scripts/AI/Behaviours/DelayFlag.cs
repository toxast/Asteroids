using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class DelayFlag : IDelayFlag {
	public bool passed{ get; private set; } 
	RandomFloat rnd;
	float timeLeft;
	public DelayFlag(bool passedAtStart, float min, float max){
		this.passed = passedAtStart;
		rnd = new RandomFloat(min, max);
		timeLeft = rnd.RandomValue;
	}

	public void Set(){
		passed = false;
		timeLeft = rnd.RandomValue;
	}

	public void SetOnMin(){
		passed = false;
		timeLeft = rnd.min;
	}

	public void Tick(float delta) {
		if (!passed) {
			timeLeft -= delta;
			if (timeLeft < 0) {
				timeLeft = rnd.RandomValue;
				passed = true;
			}
		}
	}
}
