using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class InvisibleBossSpaceshipController : InvisibleSpaceshipController{
	MInvisibleBossData bossdata;
	BossInvisibleTriggerBeh spawnMinesBeh;
	List<PolygonGameObject> spawnedObjects = new List<PolygonGameObject>();

	public InvisibleBossSpaceshipController (SpaceShip thisShip, List<PolygonGameObject> bullets, Gun gun,  MInvisibleBossData mdata) 
		: base(thisShip, bullets, gun, mdata){
	}

	protected override void InitLogic (CommonBeh.Data behData)	{
		base.InitLogic (behData);
		this.bossdata = mdata as MInvisibleBossData;
		CreateMineSpawnerBeh (behData);
		thisShip.OnDestroying += HandleDestory;
	}


	void HandleDestory(){
		foreach (var item in spawnedObjects) {
			if (!Main.IsNull (item)) {
				item.deathAnimation = null;
				item.Kill (PolygonGameObject.KillReason.EXPIRED);
			}
		}
	}

	void CreateMineSpawnerBeh(CommonBeh.Data behData){
		spawnMinesBeh = new BossInvisibleTriggerBeh (behData, new NoDelayFlag (), IsInvisibleBeh, bossdata.mineSpawn, bossdata.fighterSpawn);
		spawnMinesBeh.OnMineSpawned += HandleMineSpawned;
		spawnMinesBeh.OnFighterSpawned += SpawnMinesBeh_OnFighterSpawned;
		if (spawnMinesBeh.IsReadyToAct ()) {
			spawnMinesBeh.Start ();
		}
	}


	public override void Tick (float delta)	{
		base.Tick (delta);
		spawnMinesBeh.Tick (delta);
	}

	void HandleMineSpawned(PolygonGameObject mine) {
		Main.PutOnFirstNullPlace (spawnedObjects, mine);
		mine.invisibilityComponent = new InvisibilityComponent (mine, new InvisibilityComponent.Data{fadeInDuration = 0.5f, fadeOutDuration = 0.5f, slowerFadeOnHit = 4f});
		mine.SetAlphaAndInvisibility(0);
		mine.invisibilityComponent.SetState (false);
		Singleton<Main>.inst.Add2Objects (mine);
	}

	void SpawnMinesBeh_OnFighterSpawned (PolygonGameObject spawn)
	{
		Main.PutOnFirstNullPlace (spawnedObjects, spawn);
		spawn.invisibilityComponent = new InvisibilityComponent (spawn, new InvisibilityComponent.Data{fadeInDuration = 0.5f, fadeOutDuration = 1f, slowerFadeOnHit = 4f});
		spawn.SetAlphaAndInvisibility(0);
		spawn.invisibilityComponent.SetState (false);
		Singleton<Main>.inst.Add2Objects (spawn);
	}


	public class BossInvisibleTriggerBeh : DelayedActionBeh{
		MMineData mineSpawn;
		MSpawnDataBase fighterSpawn;
		Func<bool> isInvisibleBeh;
		public event Action<PolygonGameObject> OnMineSpawned;
		public event Action<PolygonGameObject> OnFighterSpawned;
		public BossInvisibleTriggerBeh (CommonBeh.Data data, IDelayFlag delay, Func<bool> isInvisibleBeh, MMineData mineSpawn, MSpawnDataBase fighterSpawn)
			:base(data, delay) 
		{
			this.mineSpawn = mineSpawn;
			this.fighterSpawn = fighterSpawn;
			this.isInvisibleBeh = isInvisibleBeh;
		}

		protected override IEnumerator Action ()
		{
			float interval = 0.5f;
			float intervalLeft = interval;
			bool spawnedFightersThisTime = false;
			while (true) {
				if (isInvisibleBeh ()) {
					if (!spawnedFightersThisTime && thisShip.GetLeftHealthPersentage () < 0.55f) {
						spawnedFightersThisTime = true;
						Vector3 dir = Math2d.RotateVertexDeg (thisShip.cacheTransform.right, 90f);
						SpawnFighter (dir);
						SpawnFighter (-dir);
						if (thisShip.GetLeftHealthPersentage () < 0.25f) {
							SpawnFighter (-thisShip.cacheTransform.right);
						}
					}

					intervalLeft -= DeltaTime ();
					if (intervalLeft <= 0) {
						intervalLeft += interval;
						SpawnMine ();
					}
				} else {
					spawnedFightersThisTime = false;
				}
				yield return true;
			}
		}

		private void SpawnMine(){
			var mine = mineSpawn.Create ();
			mine.position = thisShip.position;
			mine.velocity = new RandomFloat(10, 15).RandomValue * Math2d.RotateVertexDeg (-thisShip.cacheTransform.right, new RandomFloat (-90, 90).RandomValue);
			if (OnMineSpawned != null) {
				OnMineSpawned (mine);
			}
		}

		private void SpawnFighter(Vector3 dir){
			var spawn = fighterSpawn.Create();
			spawn.position = thisShip.position;
			spawn.velocity = 15f * dir;
			if (OnFighterSpawned != null) {
				OnFighterSpawned(spawn);
			}
		}
	}
}
