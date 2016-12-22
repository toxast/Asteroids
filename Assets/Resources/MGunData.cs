using UnityEngine;
using System.Collections;

public class MGunData : MGunBaseData, IGotShape {
	public float damage = 3;
	public float lifeTime = 2;
	public float bulletSpeed = 35;
	public float fireInterval = 0.5f;
	public PhysicalData physical;
	public int repeatCount = 0;
	public float repeatInterval = 0;
	public Vector2[] vertices = PolygonCreator.GetRectShape(0.4f, 0.2f); 
	public Color color = Color.red;
	public ParticleSystem fireEffect;

	public Vector2[] iverts {get {return vertices;} set{vertices = value;}}

	public override Gun GetGun(Place place, PolygonGameObject t)
	{
		return new BulletGun(place, this, t);
	}
}
