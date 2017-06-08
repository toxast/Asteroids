using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Game : MonoBehaviour 
{
	[SerializeField] Camera uiCamera;
	[SerializeField] UIMainMenu mainMenu;
	[SerializeField] GUIHangar hangar;
	[SerializeField] Main main;

	[SerializeField] List<GameObject> gameObjects;

	[Header("editor")]
	[SerializeField] InputField levelInput;
	[SerializeField] InputField waveInput;
	[SerializeField] Button addMoneyEditor;

	[SerializeField] MSpaceshipData overrideShipInEditor;
	[SerializeField] bool useAIforUser = false;

	string currentSlot;
	int currentLevelIndex;
	IntHashSave cometUnlocks;

	void Awake() {
		levelInput.gameObject.SetActive(Application.isEditor);
		waveInput.gameObject.SetActive(Application.isEditor);
		addMoneyEditor.gameObject.SetActive(Application.isEditor);

		mainMenu.OnLoadGame += HandleMainMenuStartGame;
		hangar.startTheGame += HandleStartTheGame;
		main.OnQuitToMenu += Main_OnQuitToMenu;
		main.OnGameOver += HandleGameOver;
		main.OnLevelCleared += HandlelevelCleared;

		addMoneyEditor.onClick.AddListener (() => GameResources.AddMoney(1000));
	}

	void Start() {
		ToggleUICamera (true);
		mainMenu.Show ();
	}

	void HandleMainMenuStartGame (int slot) {
		LoadGame (slot.ToString ());
	}

	public bool UseAiForUser(){
		#if UNITY_EDITOR 
		return useAIforUser;
		#else
		return false;
		#endif
	}

	void LoadGame(string slot) {
		Logger.Log ("__________________");
		Logger.Log ("load game " + slot);
		bool newGame = PlayerPrefs.GetInt (GetGameStartedSaveKey(slot), 0) == 0;
		PlayerPrefs.SetInt (GetGameStartedSaveKey(slot), 1);
		currentSlot = slot;
		GameResources.LoadMoney (GetMoneySaveKey(currentSlot));
		IntHashSave shipsSaves = new IntHashSave(GetShipsSaveKey(currentSlot), new List<int>{1});
		shipsSaves.Load ();
		cometUnlocks = new IntHashSave(GetCometsSaveKey(currentSlot));
		cometUnlocks.Load ();
		cometUnlocks.Add (100);
		IntHashSave journal = new IntHashSave(GetJournalSaveKey(currentSlot));
		journal.Load ();
		hangar.Init (shipsSaves, cometUnlocks, journal);
		hangar.Show (AvaliableLevelIndex);
		if (newGame) {
			hangar.ShowStartMessage ();
		}
		hangar.lastBoughtPowerups.Clear();
	}

	void HandleStartTheGame (MSpaceshipData shipData, int Level) {
		#if UNITY_EDITOR
		if(overrideShipInEditor != null){
			shipData = overrideShipInEditor;
		}
		#endif
		hangar.Hide ();
		ToggleUICamera (false);
		gameObjects.ForEach (h => h.SetActive (true));
		Logger.Log("                       ");
		Logger.Log ("START LEVEL: " + Level + " money: " + GameResources.money);
		AreaSizeData areaData;
		var spawner = DetermineLvel (Level, out areaData);
		main.StartTheGame (shipData, GetActiveComets(), areaData, spawner, new Queue<MCometData>(hangar.lastBoughtPowerups));
		hangar.lastBoughtPowerups.Clear();
	}
	 
	List<MCometData> GetActiveComets() {
		List<MCometData> powerups = new List<MCometData> ();
		var elemsData = MPowerUpResources.Instance.powerups;
		for (int i = 0; i < elemsData.Count; i++) {
			var upgradeList = elemsData [i].comets;
			var lastBoughtUpgrade = upgradeList.FindLast (s => cometUnlocks.Contains (s.id));
			if (lastBoughtUpgrade != null) {
				powerups.Add (lastBoughtUpgrade);
			}
		}
		return powerups;
	}

	public static string GetGameStartedSaveKey(string slot) { return slot + "game";}
	public static string GetMoneySaveKey(string slot) { return slot + "essence";}
	public static string GetShipsSaveKey(string slot) { return slot + "ships";}
	public static string GetCometsSaveKey(string slot) { return slot + "comets";}
	public static string GetLevelSaveKey(string slot) { return slot + "level";}
	public static string GetJournalSaveKey(string slot) { return slot + "journal";}

	public static void DeleteSaves(string slot){
		PlayerPrefs.SetInt (GetGameStartedSaveKey(slot), 0);
		PlayerPrefs.SetInt (GetMoneySaveKey(slot), 0);
		PlayerPrefs.SetString(GetShipsSaveKey(slot), "");
		PlayerPrefs.SetString(GetCometsSaveKey(slot), "");
		PlayerPrefs.SetString(GetJournalSaveKey(slot), "");
		PlayerPrefs.SetInt (GetLevelSaveKey (slot), 0);
	}

	void HandlelevelCleared () {
		Debug.LogWarning ("levelCleared");
		StartCoroutine (FinishGameIn (1f, true));
	}

	void ToggleUICamera(bool render) {
		uiCamera.gameObject.SetActive (render);
	}

	void HandleGameOver () {
		#if UNITY_EDITOR
		if (currentLevelIndex == -1) {
			return;
		}
		#endif
		Debug.LogWarning ("GameOver");
		StartCoroutine (FinishGameIn (3f, false));
	}

	void Main_OnQuitToMenu ()	{
		StartCoroutine (FinishGameIn (0.0001f, false, true));
	}

	IEnumerator FinishGameIn(float seconds, bool success, bool userExited = false) {
		yield return new WaitForSeconds (seconds);

		ToggleUICamera (true);
		gameObjects.ForEach (h => h.SetActive (false));

		yield return new WaitForEndOfFrame ();
		yield return new WaitForEndOfFrame ();
	
		if (currentLevelIndex >= 0 && success) {
			SaveLevelPassed (currentLevelIndex);
		}

		main.Clear ();
		hangar.Show (AvaliableLevelIndex);

		if (!userExited  && currentLevelIndex >= 0) {
			var level = MLevelsResources.Instance.levels [currentLevelIndex];
			if (level != null) {
				hangar.ShowMessage (level.journal);
				if (success) {
					hangar.ShowMessage (level.journalFinish);
				}
			}
		}

		Logger.Log("MONEY: " + GameResources.money);
	}

	int AvaliableLevelIndex{
		get{ return PlayerPrefs.GetInt (GetLevelSaveKey (currentSlot), 0); }
	}

	void SaveLevelPassed(int levelIndx){
		int toSave = levelIndx + 1;
		if (toSave > AvaliableLevelIndex) {
			Debug.LogError ("level passed: " + toSave);
			PlayerPrefs.SetInt (GetLevelSaveKey (currentSlot), toSave);
		}
	}

	ILevelSpawner DetermineLvel(int levelIndx, out AreaSizeData areaData) {
		currentLevelIndex = levelIndx;
		areaData = new AreaSizeData ();
		#if UNITY_EDITOR
		ILevelSpawner spawner;
		int level = int.Parse (levelInput.text);
		int waveNum = int.Parse (waveInput.text);
		if (level < 0) {
			currentLevelIndex = -1;
			spawner = new EmptyTestSceneSpawner ();
		} else {
			if(level == 0 && waveNum == 0){
				var lvl = MLevelsResources.Instance.levels [levelIndx];
				areaData = lvl.areaData;
				spawner = lvl.GetLevel();
				return spawner;
			} else {
				currentLevelIndex = level;
				var lvl = MLevelsResources.Instance.levels [level];
				areaData = lvl.areaData;
				spawner = lvl.GetLevel();
				//testing purposes
				if (waveNum != 0) {
					(spawner as LevelSpawner).ForceWaveNum(waveNum);
				}
			}
		} 
		return spawner;
		#else
		var lvl = MLevelsResources.Instance.levels [levelIndx];
		areaData = lvl.areaData;
		var spawner = lvl.GetLevel();
		return spawner;
		#endif
	}

	void OnApplicationPause(){
		PlayerPrefs.Save ();
	}

	void OnApplicationQuit(){
		PlayerPrefs.Save ();
	}
}
