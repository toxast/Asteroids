using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 public class MSpawnBase : MonoBehaviour {

	public virtual float difficulty{get{return 0;}}
	public virtual void Spawn(Vector2 pos, float lookAngle, Action<SpawnedObj> callback){}

	protected IEnumerator SpawnRoutine(SpawnElem elem, Vector2 origin, float range, float rangeAngle, float angleLookAtOrigin, Action<SpawnedObj> callback)
	{
		var main = Singleton<Main>.inst;
		Vector2 elemPos = origin + Math2d.RotateVertexDeg (new Vector2(range, 0) + elem.place.pos, rangeAngle);
		float elemRotationAngle = rangeAngle + Math2d.GetRotationDg (elem.place.dir) + (180 + angleLookAtOrigin);

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
}

