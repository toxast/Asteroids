using UnityEngine;
using System.Collections;

[System.Serializable]
public class ThrusterSetupData: IClonable<ThrusterSetupData>, IGotPlace
{
	public Place place;
	public ParticleSystem thrusterPrefab;

	public Place pos {get {return place;} set{place = value;}}

	public ThrusterSetupData Clone()
	{
		return new ThrusterSetupData 
		{
			place = place.Clone(),
			thrusterPrefab = thrusterPrefab,
		};
	}
}
