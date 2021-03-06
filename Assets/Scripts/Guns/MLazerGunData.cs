﻿using UnityEngine;
using System.Collections;

public class MLazerGunData : MGunBaseData
{
	[SerializeField] float totalAttackDamage;
	[Space(20)]
	public float damage = 3;
	public float attackDuration;
	public float pauseDuration;
	public Color color = Color.red;
	public ParticleSystem fireEffect;
	public float distance = 50f;
	public float width = 0.5f;
	public IceEffect.Data iceData;

	protected override float CalculateDps ()
	{
		totalAttackDamage = damage * attackDuration;
		return totalAttackDamage / (attackDuration + pauseDuration);
	}

	public override Gun GetGun(Place place, PolygonGameObject t)
	{
		return new LazerGun(place, this, t);
	}
}
