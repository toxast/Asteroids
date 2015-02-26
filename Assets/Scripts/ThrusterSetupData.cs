using UnityEngine;
using System.Collections;

[System.Serializable]
public class ThrusterSetupData: IClonable<ThrusterSetupData>
{
	public Place place;
	public ParticleSystem thrusterPrefab;

	public ThrusterSetupData Clone()
	{
		return new ThrusterSetupData 
		{
			place = place.Clone(),
			thrusterPrefab = thrusterPrefab,
		};
	}
}
