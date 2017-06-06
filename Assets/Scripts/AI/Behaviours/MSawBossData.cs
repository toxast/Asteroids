using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MSawBossData : MSawData {

	[Header ("BOSS data")]
    public RotatingObjectsShield.Data generatorsShieldData;
    
	public float spawnDuration = 6f;
	public float spawnInterval = 4f;
	public MSpawnDataBase chargeSpawn;

	[Header ("effects")]
	public ParticleSystemsData eyePS;
	public ParticleSystemsData eyeFlamePS;
	public ParticleSystemsData deathPS;

    protected override PolygonGameObject CreateInternal(int layer) {
        return ObjectsCreator.CreateSawBossEnemy(this, layer);
    }
}
