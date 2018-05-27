using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class MGunData : MGunBaseData, IGotShape {
	[Space(20)]
	public float hitDamage = 3;
	public float lifeTime = 2;
	public float velocity = 35;
	public float fireInterval = 0.5f;
	public int repeatCount = 0;
	public float repeatInterval = 0;
	public PhysicalData physical;
	public DeathData deathData;
	public float spreadAngle = 0;
	public RandomFloat rotation = new RandomFloat (0, 0);
	public Color color = Color.red;
	public BurningEffect.Data burnDOT;
	public IceEffect.Data iceData;
	public MEffectData scriptEffect;
	public ParticleSystem fireEffect;
    public List<ParticleSystemsData> effects;
	public List<ParticleSystemsData> destructionEffects;
	public Vector2[] vertices = PolygonCreator.GetRectShape(0.4f, 0.2f); 

	public Vector2[] iverts {get {return vertices;} set{vertices = value;}}

	protected override void OnValidate(){
		base.OnValidate ();
		effects.SetDefaultValues ();
		destructionEffects.SetDefaultValues ();
	}

	protected override float CalculateDps() {
		return (HitDamage() + BurnDamage())/TotalInterval();
	}

	protected override float CalculateRange ()
	{
		return velocity * lifeTime;
	}

	protected float TotalInterval(){
		float totalInterval = fireInterval;
		if (repeatCount > 0) {
			totalInterval += (repeatCount - 1) * repeatInterval;
		}
		return totalInterval;
	}

	[Space(20)]
	[SerializeField] float explosionRangeCalculated;
	[SerializeField] float explosionDamageCalculated;
	protected virtual float HitDamage () {
		collisionDmg = 0;
		float totalDamage = hitDamage;
		float explosionDamage = 0;
		if (vertices.Length > 2) {
			float area;
			Math2d.GetMassCenter (vertices, out area);
			collisionDmg = GlobalConfig.DamageFromCollisionsModifier * physical.collisionAttackModifier * PolygonCollision.GetApproximateBulletHitImpulse(this);
			explosionRangeCalculated = deathData.overrideExplosionRange >= 0 ? deathData.overrideExplosionRange : DeathAnimation.ExplosionRadius (area);
			explosionDamageCalculated = DeathAnimation.ExplosionDamage (explosionRangeCalculated);
		}
		if (deathData.createExplosionOnDeath) {
			if (deathData.overrideExplosionDamage > 0) {
				explosionDamage += deathData.overrideExplosionDamage;
			} else {
				explosionDamage += explosionDamageCalculated;
			}
		}
		totalDamage += explosionDamage;
		totalDamage += collisionDmg;
		totalDamage = totalDamage + repeatCount * totalDamage;
		return totalDamage;
	}

	protected virtual float BurnDamage(){
		float burnDps = 0;
		if (burnDOT.dps > 0 && burnDOT.maxBuildUpDuration > 0) {
			float durationB = (1 + repeatCount) * burnDOT.duration;
			durationB = Mathf.Min (durationB, burnDOT.maxBuildUpDuration);
			float totalBurnDmg = durationB * burnDOT.dps;
			burnDps = totalBurnDmg;
		}
		return burnDps;
	}


	public override Gun GetGun(Place place, PolygonGameObject t)
	{
		return new BulletGun<PolygonGameObject>(place, this, t);
	}
}
