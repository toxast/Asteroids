﻿using UnityEngine;
using System.Collections;

public class TimeDestuctor
{
	public Asteroid a;
	public float initialTime;
	public float timeLeft;

	public TimeDestuctor(Asteroid a, float timeLeft)
	{
		this.a = a;
		this.initialTime = timeLeft;
		this.timeLeft = initialTime;
	}

	public void Tick(float dtime)
	{
		timeLeft -= dtime;
		a.SetAlpha (Mathf.Clamp01 (timeLeft / initialTime));
		a.Tick (dtime);
	}

	public bool IsTimeExpired()
	{
		return timeLeft <= 0;
	}

}