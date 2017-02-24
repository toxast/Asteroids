using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBackupEffect : TickableEffect{

	protected override eType etype { get { return eType.SpawnBackup; } }
	public override bool CanBeUpdatedWithSameEffect { get { return false;} }

	protected Data data;
	bool spawned = false;

	public SpawnBackupEffect(Data data) {
		this.data = data;
	}

	public override void Tick (float delta) {
		base.Tick (delta);
		if (!spawned) {
			spawned = true;
			Spawn (data.spawns);
		}
	}

	public override bool IsFinished() {	return spawned; }

	private void Spawn(List<MRandomWave.WeightedSpawn> spawns){
		var main = Singleton<Main>.inst;
		for (int i = 0; i < spawns.Count; i++) {
			var item = spawns [i];
			Vector2 pos;
			float lookAngle;
			main.GetRandomPosition (item.range, item.positioning, out pos, out lookAngle);
			item.spawn.Spawn (pos, lookAngle, null);
		}
	}

	[System.Serializable]
	public class Data {
		public List<MRandomWave.WeightedSpawn> spawns;
	}
}