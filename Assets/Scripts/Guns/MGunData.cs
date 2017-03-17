using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class MGunData : MGunBaseData, IGotShape {
	public float hitDamage = 3;
	public float lifeTime = 2;
	public float velocity = 35;
	public float fireInterval = 0.5f;
	public int repeatCount = 0;
	public float repeatInterval = 0;
	public PhysicalData physical;
	public float spreadAngle = 0;
	public RandomFloat rotation = new RandomFloat (0, 0);
	public Color color = Color.red;
	public BurningEffect.Data burnDOT;
	public IceEffect.Data iceData;
	public ParticleSystem fireEffect;
    public List<ParticleSystemsData> effects;
	public List<ParticleSystemsData> destructionEffects;
	public Vector2[] vertices = PolygonCreator.GetRectShape(0.4f, 0.2f); 

	public Vector2[] iverts {get {return vertices;} set{vertices = value;}}

	protected virtual void OnValidate(){
		effects.SetDefaultValues ();
		destructionEffects.SetDefaultValues ();
	}

	public override Gun GetGun(Place place, PolygonGameObject t)
	{
		return new BulletGun<PolygonGameObject>(place, this, t);
	}
}
