using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class FullSpaceShipSetupData : IClonable<FullSpaceShipSetupData>, IGotShape, IGotThrusters, IGotGuns, IGotTurrets
{
	public string name;
	public int price = -1;
	public Color color = Color.white;
	public PhysicalData physical;
	public AIType ai = AIType.eCommon;
	public SpaceshipData mobility;
	public ShieldData shield;
	public List<GunSetupData> guns;
	public List<ThrusterSetupData> thrusters;
	public List<TurretReferenceData> turrets;
	public Vector2[] verts;

	//interfaces
	public Vector2[] iverts {get {return verts;} set{verts = value;}}
	public List<GunSetupData> iguns {get {return guns;} set{guns = value;}}
	public List<ThrusterSetupData> ithrusters {get {return thrusters;} set{thrusters = value;}}
	public List<TurretReferenceData> iturrets {get {return turrets;} set{turrets = value;}}

	public FullSpaceShipSetupData Clone()
	{
		FullSpaceShipSetupData r = new FullSpaceShipSetupData ();
		r.name = name + " clone";
		r.price = price;
		r.ai = ai;
		r.color = color; 
		r.physical = physical.Clone(); 
		r.mobility = mobility.Clone();
		r.shield = shield.Clone(); 
		r.guns = guns.ConvertAll(g => g.Clone());
		r.thrusters = thrusters.ConvertAll(t => t.Clone());
		r.turrets = turrets.ConvertAll(t => t.Clone());
		r.verts = verts.ToList ().ToArray ();
		return r;
	}


	public enum AIType
	{
		eCommon = 0,
		eSuicide = 1,
	}
}
