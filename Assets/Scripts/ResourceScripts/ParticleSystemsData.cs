using UnityEngine;
using System.Collections;

[System.Serializable]
public class ParticleSystemsData: IClonable<ParticleSystemsData>, IGotPlace
{
	public Place place = new Place();
	public ParticleSystem prefab;
    public float overrideSize = -1;
    public float overrideDuration = -1;
    public float zOffset = 1;

    public Place pos {get {return place;} set{place = value;}}

	public ParticleSystemsData Clone()
	{
        return new ParticleSystemsData {
            place = place.Clone(),
            prefab = prefab,
            overrideSize = overrideSize,
            overrideDuration = overrideDuration,
            zOffset = zOffset,
        };
	}
}
