using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class TurretSetupData : IClonable<TurretSetupData>, IGotShape, IGotGuns
{
	public string name = "turret";
	public Color color = Color.white;
	public float rotationSpeed = 50;
	public float restrictionAngle = 360;
	public List<GunSetupData> guns;
	public Vector2[] verts;

	//interfaces
 	public Vector2[] iverts {get {return verts;} set{verts = value;}}
	public List<GunSetupData> iguns {get {return guns;} set{guns = value;}}

	public TurretSetupData Clone()
	{
		TurretSetupData r = new TurretSetupData ();
		r.name = name + " clone";
		r.color = color; 
		r.rotationSpeed = rotationSpeed;
		r.restrictionAngle = restrictionAngle;
		r.guns = guns.ConvertAll(g => g.Clone());
		r.verts = verts.ToList ().ToArray ();
		return r;
	}
}

