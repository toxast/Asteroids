using UnityEngine;
using System.Collections;

public class MSawData : MonoBehaviour 
{
	public SawInitData mdata;

    void Awake()
    {
        if (Application.isPlaying && Application.isEditor) {
            var obj = Create ();
            obj.cacheTransform.position = gameObject.transform.position;

            if(obj.layerNum != CollisionLayers.ilayerAsteroids)
                obj.targetSystem = new TargetSystem (obj);

            Singleton<Main>.inst.Add2Objects(obj);
        }
    }

    public PolygonGameObject Create()
    {
        return ObjectsCreator.CreateSawEnemy(mdata);
    }
}
