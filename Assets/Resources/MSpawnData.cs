using UnityEngine;
using System.Collections;

public class MSpawnData<T> : MonoBehaviour, ISwanable
	where T: PolygonGameObject
{
	[Header("editor fields")]
	public int spawnCount = 1;
	public bool spawn = true;
	public int editorSpawnLayer = CollisionLayers.ilayerTeamEnemies;
	[Space(20)]
	public int _spaceField;

	#if UNITY_EDITOR
	void Update() {
		if (spawn && Application.isPlaying && Application.isEditor) {
			spawn = false;
			for (int i = 0; i < spawnCount; i++) {
				Spawn ();
			}
		}
	}
	#endif

    public PolygonGameObject CreatePolygonGO() {
        return Create ();
    }

	private void Spawn() {
		var obj = Create ();

		if(obj.layerNum != CollisionLayers.ilayerAsteroids)
			obj.targetSystem = new TargetSystem (obj);

		Vector2 pos;
		float lookAngle;
		SpawnPositioning positioning = new SpawnPositioning ();
		Singleton<Main>.inst.GetRandomPosition(new Vector2(40, 50), positioning, out pos, out lookAngle);

		obj.cacheTransform.position = pos;
		obj.cacheTransform.rotation = Quaternion.Euler (0, 0, lookAngle);

		Singleton<Main>.inst.Add2Objects(obj);
	}

	public virtual T Create (){return null;}
}

public interface ISwanable {
    PolygonGameObject CreatePolygonGO ();
}
