using UnityEngine;
using System.Collections;

[System.Serializable]
public class RocketLauncherData : IClonable<RocketLauncherData>
{
	public GunData baseData;
	public SpaceshipData missleParameters;
	public ParticleSystem thrusterEffect;
	public Vector3 thrusterPos;

	public RocketLauncherData Clone()
	{
		RocketLauncherData r = new RocketLauncherData ();
		r.baseData = baseData.Clone ();
		r.missleParameters = missleParameters.Clone ();
		r.thrusterEffect = thrusterEffect; 
		r.thrusterPos = thrusterPos;
		return r;
	}
}
