using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class IntHashSave{
	string savekey;
	HashSet<int> data = new HashSet<int>{};
	List<int> initList = new List<int>();

	//this saves will always have init list values
	public IntHashSave(string savekey, List<int> init = null){
		this.savekey = savekey;
		if (init != null) {
			initList.AddRange (init);
		}
		for (int i = 0; i < initList.Count; i++) {
			data.Add (initList [i]);
		}
	}

	void Save() {
		string saveString = string.Empty;
		var list = data.ToList ();
		for (int i = 0; i < list.Count; i++) {
			saveString += list[i].ToString() + " ";
		}
		PlayerPrefs.SetString (savekey, saveString);
	}

	public bool Contains(int id){
		return data.Contains (id);
	}

	public void Add(int id){
		data.Add (id);
		Save ();
	}

	public void Load() {
		string loadedString = PlayerPrefs.GetString (savekey, "");
		var parts = loadedString.Split (' ');
		for (int i = 0; i < parts.Count(); i++) {
			int res;
			if(System.Int32.TryParse(parts[i], out res)){
				data.Add (res);
			}
		}
	}

	public void DeleteAllSaves() {
		data = new HashSet<int>{};
		for (int i = 0; i < initList.Count; i++) {
			data.Add (initList [i]);
		}
		Save ();
	}
}