using UnityEngine;
using System.Collections;

public class MSpawnData<T> : MonoBehaviour, ISwanable
	where T: PolygonGameObject
{
	public virtual void Awake()
	{
		if (Application.isPlaying && Application.isEditor) 
		{
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
	}
    public PolygonGameObject CreatePolygonGO() {
        return Create ();
    }

	public virtual T Create (){return null;}
}

public interface ISwanable {
    PolygonGameObject CreatePolygonGO ();
}
