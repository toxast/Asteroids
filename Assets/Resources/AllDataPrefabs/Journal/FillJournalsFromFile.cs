using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class FillJournalsFromFile : MonoBehaviour {

	[SerializeField] bool fillJournals = false;	
	// Update is called once per frame
	void Update () {
		if (fillJournals) {
			fillJournals = false;
			FillJournals ();
		}
	}

	void FillJournals() {
		#if UNITY_EDITOR
		var journals = EditorHelper.LoadPrefabsContaining<MJournalLog> ();
		MJournalLog journal = null;
		System.IO.StreamReader file = new System.IO.StreamReader (Application.dataPath +"/Plot.log");
		string line = string.Empty;
		int id;
		string msg = "";
		while ((line = file.ReadLine()) != null){
			if (int.TryParse (line, out id)) {
				if (journal != null) {
					Finalise(journal, msg);
					msg = "";
				}
				journal = journals.Find (j => j.id == id);
				if(journal != null){
					msg += "id " + id + ": " + journal.name;
					journal.entryName = file.ReadLine ();
					msg +="\nname: " + journal.entryName; 
					journal.text = "";
				} else {
					Debug.LogError("journal not found by id: " + id);
				}
			} else if(journal != null){
				journal.text += line;
				EditorUtility.SetDirty (journal.gameObject);
			} else {
				Debug.LogError("line lost: " + line);
			}
		}
		if(journal != null){
			Finalise(journal, msg);
		}

		file.Close ();
		AssetDatabase.SaveAssets ();
		#endif
	}

	void Finalise(MJournalLog log, string msg){
		log.text = log.text.TrimEnd (' ', '\n');
		msg += log.text;
		Debug.LogWarning(msg);
		#if UNITY_EDITOR
		EditorUtility.SetDirty (log.gameObject);
		#endif
	}
}
