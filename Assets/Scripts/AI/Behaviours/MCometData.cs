using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MCometData : MSpawnDataBase, IGotShape {
	public int id;
	public int price;
	public MJournalLog journal;
	public MSpaceshipData shipRestricltion; // not avaliable unlit ship is bought

	[Header ("drop from enemies")]
	public bool dropFromEnemies = false;
	public int dropCountPerLevel = 0;

	[Header ("other")]
	public PowerupData powerupData;
	public Vector2[] iverts {get {return powerupData.verts;} set{powerupData.verts = value;}}

	protected override PolygonGameObject CreateInternal(int layer) {
		float speed = 20f;
		var spawn = ObjectsCreator.CreateComet(this, new RandomFloat(speed * 0.4f, speed * 0.7f), 120f);
        return spawn;
    }

	public PolygonGameObject GameCreate(float speed, float lifetime){
		var spawn = ObjectsCreator.CreateComet(this, new RandomFloat(speed * 0.4f, speed * 0.7f), lifetime);
		return spawn;
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