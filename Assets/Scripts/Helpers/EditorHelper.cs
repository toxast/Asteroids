using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class EditorHelper : MonoBehaviour {
	public static string[] GetAllPrefabPaths () {
		#if UNITY_EDITOR
		string[] temp = AssetDatabase.GetAllAssetPaths();
		List<string> result = new List<string>();
		foreach ( string s in temp ) {
			if ( s.Contains( ".prefab" ) ) result.Add( s );
		}
		return result.ToArray();
		#else
		return new string[1];
		#endif

	}

	public static List<T> LoadPrefabsContaining<T>() where T : UnityEngine.Component
	{
		List<T> result = new List<T>();
		var allPrefabs = GetAllPrefabPaths ();
		foreach (var path in allPrefabs)
		{
			int resIndx = path.IndexOf ("Resources");
			if (resIndx < 0)
				continue;

			string newpath = path.Substring (resIndx + "Resources/".Length );
			newpath = newpath.Substring (0, newpath.Length - ".prefab".Length);

			var cmp = Resources.Load<T>(newpath);
			if (cmp != null)
			{
				Debug.LogWarning (newpath);
				result.Add(cmp);
			}
		}
		return result;
	}

	[ContextMenu ("CustomAction")]
	public void MyCustomAction() {
		//
		var list3 = LoadPrefabsContaining<MJournalLog> ();
		int id = 0;
		foreach (var item in list3) {
			id++;
			//item.powerupData.effectData.color = item.color;
			item.id = id;
			#if UNITY_EDITOR
			EditorUtility.SetDirty (item.gameObject);
			#endif
		}
		//
		//		var list4 = LoadPrefabsContaining<MFlamerGunData> ();
		//		foreach (var item in list4) {
		//			item.deathData.destructionType = PolygonGameObject.DestructionType.eDisappear;
		//			item.deathData.createExplosionOnDeath = false;
		//			EditorUtility.SetDirty (item.gameObject);
		//		}
		//
		//		var list5 = LoadPrefabsContaining<MFlamerGunData> ();
		//		foreach (var item in list5) {
		//			item.deathData.destructionType = PolygonGameObject.DestructionType.eDisappear;
		//			item.deathData.createExplosionOnDeath = false;
		//			EditorUtility.SetDirty (item.gameObject);
		//		}
		//
		//		var list = LoadPrefabsContaining<MRocketGunData> ();
		//		foreach (var item in list) {
		//			item.deathData.destructionType = PolygonGameObject.DestructionType.eDisappear;
		//			item.deathData.createExplosionOnDeath = true;
		//			item.deathData.instantExplosion = true;
		//			item.deathData.overrideExplosionDamage = item.overrideExplosionDamage;
		//			item.deathData.overrideExplosionRange= item.overrideExplosionRadius;
		//			EditorUtility.SetDirty (item.gameObject);
		//		}
		//
		//		var list2 = LoadPrefabsContaining<MMinesGunData> ();
		//		foreach (var item in list2) {
		//			item.deathData.destructionType = PolygonGameObject.DestructionType.eComplete;
		//			item.deathData.createExplosionOnDeath = true;
		//			item.deathData.instantExplosion = true;
		//			item.deathData.overrideExplosionDamage = item.overrideExplosionDamage;
		//			item.deathData.overrideExplosionRange= item.overrideExplosionRadius;
		//			EditorUtility.SetDirty (item.gameObject);
		//		}
		//

		//AssetDatabase.SaveAssets();
	}
}
