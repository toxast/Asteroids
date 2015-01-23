using UnityEngine;
using System.Collections;

public class Singleton<T> where T: MonoBehaviour
{
	private static T _instance;
	public static T inst
	{
		get {
			if(_instance == null)
			{
				_instance = Instantiate();
			}
			return _instance;
		}
	}

	static T Instantiate() {
		var type = typeof(T);
		var objects = UnityEngine.Object.FindObjectsOfType(type);
		if ( objects != null && objects.Length > 1 )
			Debug.LogError("Present more than one singleton instance of type " + type.Name + " on scene!");
		
		var instance = objects != null && objects.Length > 0 ? objects[0] as T : null;
		if ( instance == null) {
			Debug.Log("Create singleton for type: " + type.Name);
			
			var obj = new GameObject(string.Format("_singleton_{0} - delete me in offline mode", type.Name));
			UnityEngine.Object.DontDestroyOnLoad(obj);
			
			instance = obj.AddComponent<T>();
		}
		
		return instance;
	}

}
