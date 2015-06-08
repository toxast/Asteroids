using UnityEngine;
using System.Collections;

[System.Serializable]
public class RocketLauncherData : IClonable<RocketLauncherData>, IGun, IGotShape
{
	public string name;
	public GunData baseData;
	public SpaceshipData missleParameters;
	public float accuracy = 0.5f;
	public ParticleSystem thrusterEffect;
	public Vector3 thrusterPos;

	public Vector2 launchDirection;
	public float launchSpeed;

	public string iname{ get {return baseData.name;}}
	public Vector2[] iverts {get {return baseData.vertices;} set{baseData.vertices = value;}}
	public int iprice{ get {return baseData.price;}}
	public GunSetupData.eGuns itype{ get {return GunSetupData.eGuns.ROCKET;}}

	public RocketLauncherData Clone()
	{
		RocketLauncherData r = new RocketLauncherData ();
		r.baseData = baseData.Clone ();
		r.missleParameters = missleParameters.Clone ();
		r.accuracy = accuracy;
		r.thrusterEffect = thrusterEffect; 
		r.thrusterPos = thrusterPos;
		r.launchDirection = launchDirection;
		r.launchSpeed = launchSpeed;
		return r;
	}
}
