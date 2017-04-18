using System.Collections;
using System.Collections.Generic;
using System;
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

	private void Spawn(List<SpawnPos> spawns){
		var main = Singleton<Main>.inst;
		for (int i = 0; i < spawns.Count; i++) {
			var item = spawns [i];
			item.spawn.Spawn (main.GetPositionData(item.range, item.positioning) , null);
		}
	}

	[System.Serializable]
	public class Data : IApplyable {
		public List<SpawnPos> spawns;
		public IHasProgress Apply(PolygonGameObject picker) {
			picker.AddEffect (new SpawnBackupEffect (this));
			return null;
		}
	}
}



