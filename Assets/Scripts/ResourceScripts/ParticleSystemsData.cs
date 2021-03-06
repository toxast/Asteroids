﻿using UnityEngine;
using System.Collections;

[System.Serializable]
public class ParticleSystemsData: IClonable<ParticleSystemsData>, IGotPlace
{
	public Place place = new Place();
	public ParticleSystem prefab;
	public float overrideSize = -1;
	public float overrideDelay = -1;
    public float overrideDuration = -1;
    public float zOffset = 1;
	[Header ("behaviour after holder destruction")]
	public bool afterlife = false;
	public bool stopEmission = true;
	public bool inheritVelocity = true; //conflicts with parentToSplitParts, parentToSplitParts will prevail
	public bool parentToSplitParts = false;
	[Header ("color")]
	public bool overrideStartColor = false;
	public Color startColor = Color.white;

    public Place pos {get {return place;} set{place = value;}}

	public ParticleSystemsData Clone()
	{
        return new ParticleSystemsData {
            place = place.Clone(),
            prefab = prefab,
            overrideSize = overrideSize,
			overrideDelay = overrideDelay,
			overrideDuration = overrideDuration,
			zOffset = zOffset,
			afterlife = afterlife,
			stopEmission = stopEmission,
			inheritVelocity = inheritVelocity,
			parentToSplitParts = parentToSplitParts,
			overrideStartColor = overrideStartColor,
			startColor = startColor,
        };
	}
}
