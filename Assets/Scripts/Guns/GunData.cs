using UnityEngine;
using System.Collections;
using System.Linq;

[System.Serializable]
public class GunData : IClonable<GunData>, IGun
{
	public string name;
	public int price;
	public float damage = 3;
	public float lifeTime = 2;
	public float bulletSpeed = 35;
	public float fireInterval = 0.5f;
	public int repeatCount = 0;
	public float repeatInterval = 0;
	public Vector2[] vertices = PolygonCreator.GetRectShape(0.4f, 0.2f); 
	public Color color = Color.red;
	public ParticleSystem fireEffect;

	public string iname{ get {return name;}}
	public int iprice{ get {return price;}}
	public GunSetupData.eGuns itype{ get {return GunSetupData.eGuns.BULLET;}}

	public GunData Clone()
	{
		GunData g = new GunData ();
		g.name = name;
		g.price = price;
		g.damage = damage;
		g.lifeTime = lifeTime;
		g.bulletSpeed = bulletSpeed;
		g.fireInterval = fireInterval;
		g.vertices = vertices.ToList ().ToArray ();
		g.color = color;
		g.fireEffect = fireEffect;
		g.repeatCount = repeatCount;
		g.repeatInterval = repeatInterval;
		return g;
	}
}
