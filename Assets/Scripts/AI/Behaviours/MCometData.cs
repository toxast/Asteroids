using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MCometData : MSpawnDataBase {
    public RandomFloat speed;
    public RandomFloat rotation;
    public RandomFloat size;
    public PhysicalData physical;
    public float lifeTime;
    public Color color;
	public PowerupData powerupData;
	public List<ParticleSystemsData> particles;
	public List<ParticleSystemsData> destructionEffects;
	protected override PolygonGameObject CreateInternal(int layer) {
        var spawn = ObjectsCreator.CreateComet(this);
        return spawn;
    }

	protected void OnValidate () {
		particles.SetDefaultValues ();
		destructionEffects.SetDefaultValues ();
	}
}

[System.Serializable]
public class PowerupData {
	public int cost;
	public UserComboPowerup effectData;
	public Vector2[] verts;
    public float lifeTime;
    public Color color;
	public ParticleSystemsData particles { get { return MParticleResources.Instance.powerUpDropParticles.data;} } 
    public Color particleSystemColor;
}