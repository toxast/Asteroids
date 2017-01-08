using UnityEngine;
using System.Collections;

[System.Serializable]
public class ParticleSystemsData: IClonable<ParticleSystemsData>, IGotPlace
{
	public Place place;
	public ParticleSystem prefab;

	public Place pos {get {return place;} set{place = value;}}

	public ParticleSystemsData Clone()
	{
		return new ParticleSystemsData 
		{
			place = place.Clone(),
			prefab = prefab,
		};
	}
}
