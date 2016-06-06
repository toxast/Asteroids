using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class FullSpaceShipSetupData : IClonable<FullSpaceShipSetupData>, IGotShape, IGotThrusters
{
	public string name;
	public int price = -1;
	public int reward = 0;
	public Color color = Color.white;
	public PhysicalData physical;
	public AIType ai = AIType.eCommon;
	public SpaceshipData mobility;
	public AccuracyData accuracy;
	public ShieldData shield;
	public List<GunSetupData> guns;
	public List<int> linkedGuns;
	public List<ThrusterSetupData> thrusters;
	public List<TurretReferenceData> turrets;
	public Vector2[] verts;
	public int upgradeIndex;

	//interfaces
	public Vector2[] iverts {get {return verts;} set{verts = value;}}
	public List<ThrusterSetupData> ithrusters {get {return thrusters;} set{thrusters = value;}}
	public List<TurretReferenceData> iturrets {get {return turrets;} set{turrets = value;}}

	public FullSpaceShipSetupData Clone()
	{
		FullSpaceShipSetupData r = new FullSpaceShipSetupData ();
		r.name = name + " clone";
		r.price = price;
		r.reward = reward;
		r.ai = ai;
		r.color = color; 
		r.physical = physical.Clone(); 
		r.mobility = mobility.Clone();
		r.accuracy = accuracy.Clone();
		r.shield = shield.Clone(); 
		r.guns = guns.ConvertAll(g => g.Clone());
		r.linkedGuns = new List<int> (linkedGuns);
		r.thrusters = thrusters.ConvertAll(t => t.Clone());
		r.turrets = turrets.ConvertAll(t => t.Clone());
		r.verts = verts.ToList ().ToArray ();
		r.upgradeIndex = upgradeIndex;
		return r;
	}


}
