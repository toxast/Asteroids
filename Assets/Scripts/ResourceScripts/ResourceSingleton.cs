using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class ResourceSingleton<T> : MonoBehaviour 
	where T : class
{
	[System.NonSerialized]
	static T _instance;
	
	public static T Instance
	{
		get
		{
			if(_instance == null)
			{
				var type = typeof(T);
				var objects = UnityEngine.Object.FindObjectsOfType(type);
				if (objects != null && objects.Length > 1) {
					Debug.LogError ("Present more than one singleton instance of type " + type.Name + " on scene!");
				}
				var instance = (objects != null && objects.Length > 0) ? objects[0] as T : null;
				if (instance == null) {
					var t = typeof(T);
					var obj = Resources.Load ("Prefabs/" + t.ToString (), t);
					_instance = obj as T;
				}
			}
			return _instance;
		}
	}
}