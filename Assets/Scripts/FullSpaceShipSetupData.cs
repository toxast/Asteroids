using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class FullSpaceShipSetupData
{
	public string name;
	public GlobalConfig.eLayer layer = GlobalConfig.eLayer.TEAM_ENEMIES;
	public Color color = Color.white;
	public SpaceshipData physicalParameters;
	public List<GunSetupData> guns;
	public List<ThrusterSetupData> thrusters;
	public Vector2[] verts;
}
