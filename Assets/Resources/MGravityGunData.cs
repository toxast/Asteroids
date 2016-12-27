using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MGravityGunData : MGunBaseData, IGotShape {
    public float damagePerSecond = 3;
    public float force = 10;
    public float range = 20;
    public float lifeTime = 5;
    public float bulletSpeed = 35;
    public float fireInterval = 3f;
    public int repeatCount = 0;
    public float repeatInterval = 0;
    public Vector2[] vertices = PolygonCreator.GetRectShape(0.4f, 0.2f); 
    public Color color = Color.magenta;
    public ParticleSystem fireEffect;
    public ParticleSystem bulletEffect;

    public Vector2[] iverts {get {return vertices;} set{vertices = value;}}

    public override Gun GetGun(Place place, PolygonGameObject t)
    {
        return new GravityGun(place, this, t);
    }
}
