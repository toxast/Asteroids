using UnityEngine;
using System.Collections;

[System.Serializable]
public class RocketLauncherData : IClonable<RocketLauncherData>
{
	public string name;
	public GunData baseData;
	public SpaceshipData missleParameters;
	public ParticleSystem thrusterEffect;
	public Vector3 thrusterPos;

	public Vector2 launchDirection;
	public float launchSpeed;

	public RocketLauncherData Clone()
	{
		RocketLauncherData r = new RocketLauncherData ();
		r.baseData = baseData.Clone ();
		r.missleParameters = missleParameters.Clone ();
		r.thrusterEffect = thrusterEffect; 
		r.thrusterPos = thrusterPos;
		r.launchDirection = launchDirection;
		r.launchSpeed = launchSpeed;
		return r;
	}
}
