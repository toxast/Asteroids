using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class TowerSetupData : IClonable<TowerSetupData>, IGotShape, IGotGuns, IGotTurrets
{
	public string name = "tower";
	public float density = 1f;
	public float healthModifier = 1f;
	public Color color = Color.white;
	public float rotationSpeed = 50;
	public ShieldData shield;
	public List<GunSetupData> guns;
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
		r.density = density; 
		r.healthModifier = healthModifier; 
		r.color = color; 
		r.rotationSpeed = rotationSpeed;
		r.shield = shield.Clone();
		r.guns = guns.ConvertAll(g => g.Clone());
		r.turrets = turrets.ConvertAll(t => t.Clone());
		r.verts = verts.ToList().ToArray();
		return r;
	}
}


