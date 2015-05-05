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
				var t =  typeof(T);
				var obj = Resources.Load("Prefabs/" + t.ToString(), t);
				_instance = obj as T;
			}
			return _instance;
		}
	}
}