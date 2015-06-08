using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class TowerSetupData : IClonable<TowerSetupData>, IGotShape, IGotGuns, IGotTurrets
{
	public string name = "tower";
	public int reward;
	public PhysicalData physical;
	public Color color = Color.white;
	public float shootAngle = 20;
	public float rotationSpeed = 50;
	public AccuracyData accuracy;
	public ShieldData shield;
	public List<GunSetupData> guns;
	public List<int> linkedGuns;
	public List<TurretReferenceData> turrets;
	public Vector2[] verts;
	
	//interfaces
	public Vector2[] iverts {get {return verts;} set{verts = value;}}
	public List<GunSetupData> iguns {get {return guns;} set{guns = value;}}
	public List<TurretReferenceData> iturrets {get {return turrets;} set{turrets = value;}}

	public TowerSetupData Clone()
	{
		TowerSetupData r = new TowerSetupData ();
		r.name = name + " clone";
		r.reward = reward;
		r.physical = physical.Clone(); 
		r.color = color; 
		r.shootAngle = shootAngle;
		r.rotationSpeed = rotationSpeed;
		r.accuracy = accuracy.Clone ();
		r.shield = shield.Clone();
		r.guns = guns.ConvertAll(g => g.Clone());
		r.linkedGuns = new List<int> (linkedGuns);
		r.turrets = turrets.ConvertAll(t => t.Clone());
		r.verts = verts.ToList().ToArray();
		return r;
	}
}


