using UnityEngine;
using System.Collections.Generic;

public static class MyExtensions
{
	public class ClassType<T>{
		public T val;
	}

	public static string FormString<T>(List<T> items){
		string s= string.Empty;
		for (int i = 0; i < items.Count; i++) {
			s += items [i].ToString ();
			if (i != items.Count - 1) {
				s += ", ";
			}
		}
		return s;
	}
//	public static void SetCollisionLayerNum(this PolygonGameObject g, int layerNum)
//	{
//		g.layer = 1 << layerNum;
//		g.collision = GlobalConfig.GetLayerCollisions (layerNum);
//	}
}
