using UnityEngine;
using System.Collections;

[System.Serializable]
public class GunData
{
	public float damage = 3;
	public float lifeTime= 2;
	public float bulletSpeed = 35;
	public float fireInterval = 0.5f;
	public Vector2[] vertices = PolygonCreator.GetRectShape(0.4f, 0.2f); 
	public Color color = Color.red;
	public ParticleSystem fireEffect;
}
