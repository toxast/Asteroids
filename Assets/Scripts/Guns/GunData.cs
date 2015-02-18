using UnityEngine;
using System.Collections;
using System.Linq;

[System.Serializable]
public class GunData : IClonable<GunData>
{
	public float damage = 3;
	public float lifeTime= 2;
	public float bulletSpeed = 35;
	public float fireInterval = 0.5f;
	public Vector2[] vertices = PolygonCreator.GetRectShape(0.4f, 0.2f); 
	public Color color = Color.red;
	public ParticleSystem fireEffect;

	public GunData Clone()
	{
		GunData g = new GunData ();
		g.damage = damage;
		g.lifeTime = lifeTime;
		g.bulletSpeed = bulletSpeed;
		g.fireInterval = fireInterval;
		g.vertices = vertices.ToList ().ToArray ();
		g.color = color;
		g.fireEffect = fireEffect;
		return g;
	}
}
