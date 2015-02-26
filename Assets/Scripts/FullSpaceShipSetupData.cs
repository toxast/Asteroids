using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class FullSpaceShipSetupData : IClonable<FullSpaceShipSetupData>
{
	public string name;
	public GlobalConfig.eLayer layer = GlobalConfig.eLayer.TEAM_ENEMIES;
	public Color color = Color.white;
	public SpaceshipData physicalParameters;
	public ShieldData shield;
	public List<GunSetupData> guns;
	public List<ThrusterSetupData> thrusters;
	public Vector2[] verts;


	public FullSpaceShipSetupData Clone()
	{
		FullSpaceShipSetupData r = new FullSpaceShipSetupData ();
		r.name = name + " clone";
		r.layer = layer;
		r.color = color; 
		r.physicalParameters = physicalParameters.Clone();
		r.shield = shield.Clone(); 
		r.guns = guns.ConvertAll(g => g.Clone());
		r.thrusters = thrusters.ConvertAll(t => t.Clone());
		r.verts = verts.ToList ().ToArray ();
		return r;
	}
}
