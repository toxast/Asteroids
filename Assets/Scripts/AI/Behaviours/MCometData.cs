using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MCometData : MSpawnDataBase, IGotShape {
	public int id;
	public int price;
	public MJournalLog journal;
    public RandomFloat rotation;
    public RandomFloat size;
    public PhysicalData physical;

	[Header ("drop from enemies")]
	public bool dropFromEnemies = false;
	public int dropCountPerLevel = 0;

	[Header ("other")]
//    public float lifeTime;
    public Color color;
	public PowerupData powerupData;
	public List<ParticleSystemsData> particles;
	public List<ParticleSystemsData> destructionEffects;
	public Vector2[] iverts {get {return powerupData.verts;} set{powerupData.verts = value;}}

	protected override PolygonGameObject CreateInternal(int layer) {
		float speed = 30f;

		var spawn = ObjectsCreator.CreateComet(this, new RandomFloat(speed * 0.7f, speed * 1.2f), 120f);
        return spawn;
    }

	public PolygonGameObject GameCreate(float speed, float lifetime){
		var spawn = ObjectsCreator.CreateComet(this, new RandomFloat(speed * 0.6f, speed * 0.8f), lifetime);
		return spawn;
	}

	protected void OnValidate () {
		particles.SetDefaultValues ();
		destructionEffects.SetDefaultValues ();
	}
}

[System.Serializable]
public class PowerupData {
	public float lifeTime;
	public Vector2[] verts;
	public UserComboPowerup effectData;
	public Color particleSystemColor;
	public ParticleSystemsData particles { get { return MParticleResources.Instance.powerUpDropParticles.data;} } 
}