using UnityEngine;
using System.Collections;

[System.Serializable]
public class RocketLauncherData : GunData
{
	public SpaceshipData missleParameters;
	public ParticleSystem thrusterEffect;
	public Vector3 thrusterPos;
}
