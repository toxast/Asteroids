using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 public class MSpawnBase : MonoBehaviour {

	public virtual float difficulty{get{return 0;}}
	public virtual void Spawn(PositionData data, Action<SpawnedObj> callback){}

	protected IEnumerator SpawnRoutine(SpawnElem elem, PositionData data, Action<SpawnedObj> callback)
	{
		var main = Singleton<Main>.inst;
		Vector2 elemOrigin = data.origin + Math2d.RotateVertexDeg (new Vector2 (data.range, 0), data.rangeAngle);
		float elemRotationAngle = data.rangeAngle + (180 + data.angleLookAtOrigin);
		Vector2 elemOffset = Math2d.RotateVertexDeg (elem.place.pos, elemRotationAngle);
		elemRotationAngle += Math2d.GetRotationDg (elem.place.dir);
		Vector2 elemPos = elemOrigin + elemOffset;

		//animation
		var anim = main.CreateTeleportationRing(elemPos, elem.teleportationColor, elem.teleportRingSize);
		yield return new WaitForSeconds(elem.teleportDuration);
		anim.Stop ();
		main.PutObjectOnDestructionQueue (anim.gameObject, 5f);

		PolygonGameObject obj = null;
		if (elem.prefab != null) {
			var mdata  = elem.prefab as ISpawnable;
			if (mdata != null) {
				obj = mdata.Create ();
			}
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

	[System.Serializable]
	public class SpawnElem{
		public MSpawnDataBase prefab;
		public float teleportDuration = 1.5f;
		public float teleportRingSize = 10f;
		public Color teleportationColor = new Color (1, 174f / 255f, 0);
		public Place place;
		public float difficulty {
			get{ return prefab.difficulty;}
		}
	}

	public class SpawnedObj
	{
		public PolygonGameObject obj;
		public float difficulty;
	}

	public class PositionData{
		public Vector2 origin;
		public float range;
		public float rangeAngle;
		public float angleLookAtOrigin;
	}
}

