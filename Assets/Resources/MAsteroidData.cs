using UnityEngine;
using System.Collections;

public class MAsteroidData : MonoBehaviour
{
	public RandomFloat speed;
	public RandomFloat rotation;
	public RandomFloat size;

	public MAsteroidCommonData commonData;

	void Awake()
	{
		if (Application.isPlaying && Application.isEditor) {
			var obj = Create ();
			obj.cacheTransform.position = gameObject.transform.position;
			//obj.cacheTransform.rotation = Quaternion.Euler (0, 0, lookAngle);
			Singleton<Main>.inst.Add2Objects(obj);
		}
	}

	public Asteroid Create()
	{
		return ObjectsCreator.CreateAsteroid (this);
	}
}
