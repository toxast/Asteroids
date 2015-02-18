using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class FullSpaceShipSetupData
{
	public Vector2[] verts;
	public SpaceshipData physicalParameters;
	public List<GunSetupData> guns;
	public List<ThrusterSetupData> thrusters;

}
