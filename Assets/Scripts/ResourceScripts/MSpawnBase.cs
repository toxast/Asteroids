using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 public  abstract class MSpawnBase : MonoBehaviour {
	public abstract float sdifficulty{ get;}
	public abstract void Spawn (PositionData data, Action<SpawnedObj> callback);

	public static IEnumerator SpawnRoutine(MSpawnDataBase elem, PositionData data, Place place, Action<SpawnedObj> callback)
	{
        //Debug.LogError(data.rangeAngle);
		var main = Singleton<Main>.inst;
		Vector2 elemOrigin = data.origin + Math2d.RotateVertexDeg (new Vector2 (data.range, 0), data.rangeAngle);
		float elemRotationAngle = data.rangeAngle + (180 + data.angleLookAtOrigin);
		Vector2 elemOffset = Math2d.RotateVertexDeg (place.position, elemRotationAngle);
		elemRotationAngle += Math2d.GetRotationDg (place.dir);
		Vector2 elemPos = elemOrigin + elemOffset;

		//animation
		var anim = main.CreateTeleportationRing(elemPos, elem.teleportData.color, elem.teleportData.ringSize);
		yield return new WaitForSeconds(elem.teleportData.duration);
		anim.Stop ();
		main.PutObjectOnDestructionQueue (anim.gameObject, 5f);

		PolygonGameObject obj = null;
		if (elem != null) {
			obj = elem.Create ();
		}

		if (obj != null) {
			obj.cacheTransform.position = elemPos;
			obj.cacheTransform.rotation = Quaternion.Euler (0, 0, elemRotationAngle);
			Singleton<Main>.inst.Add2Objects (obj);
		} else {
			Debug.LogError ("obj is null");
		}
		if (callback != null) {
			callback (new SpawnedObj{ obj = obj, difficulty = elem.difficulty });
		}
	}

	public class SpawnedObj {
		public PolygonGameObject obj;
		public float difficulty;
	}

	public class PositionData {
		public Vector2 origin; //origin point
		public float range; //rotate range by rangeAlgle to get next point
		public float rangeAngle;
		public float angleLookAtOrigin; //rotate objects group by angleLookAtOrigin, zero is to look at origin
	}

	[System.Serializable]
	public class TeleportData {
		public float duration = 1.5f;
		public float ringSize = 10f;
		public Color color = new Color (1, 174f / 255f, 0);
	}

}

[Serializable]
public class SpawnBase
{
	[SerializeField] public MSpawnBase spawn;
	[SerializeField] public RandomFloat range = new RandomFloat(40, 50);
	[SerializeField] public bool spawnAtViewEdge = false;
	public float difficulty{
		get{ return spawn.sdifficulty;}
	}
} 

[Serializable]
public class WeightedSpawn : SpawnBase
{
	[SerializeField] public float weight = 4;
	[SerializeField] public bool overridePositioning = false;
	[SerializeField] public SpawnPositioning positioning;
} 

[Serializable]
public class SpawnPos : SpawnBase
{
	[SerializeField] public SpawnPositioning positioning = new SpawnPositioning {positionAngleRange = 360} ;
} 


[Serializable]
public class SpawnPositioning
{
	public float positionAngle = 0f; // 0 - right, 90 - up, 180 - left, 270 - down from the user ship
	public float positionAngleRange = 0; //randomize positionAngle by this degrees
	public float lookAngle = 0f; // 0 - towards user, 180 - turn back 2 user
	public float lookAngleRange = 0; // randomize lookAngle by this degrees
}

public interface ILevelSpawner
{
	bool Done ();
	void Tick ();
}
public interface IWaveSpawner
{
	bool Done ();
	void Tick ();
}


public class EmptyTestSceneSpawner : ILevelSpawner
{
	public bool Done()
	{
		return false;
	}

	public void Tick()
	{
	}
}

