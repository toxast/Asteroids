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

	string currentSlot;
	int currentLevelIndex;
	IntHashSave cometUnlocks;

	void Awake() {
		mainMenu.OnLoadGame += HandleMainMenuStartGame;
		hangar.startTheGame += HandleStartTheGame;
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

	void LoadGame(string slot) {
		bool newGame = PlayerPrefs.GetInt (GetGameStartedSaveKey(slot), 0) == 0;
		PlayerPrefs.SetInt (GetGameStartedSaveKey(slot), 1);
		currentSlot = slot;
		GameResources.LoadMoney (GetMoneySaveKey(currentSlot));
		IntHashSave shipsSaves = new IntHashSave(GetShipsSaveKey(currentSlot), new List<int>{1});
		shipsSaves.Load ();
		cometUnlocks = new IntHashSave(GetCometsSaveKey(currentSlot));
		cometUnlocks.Load ();
		IntHashSave journal = new IntHashSave(GetJournalSaveKey(currentSlot));
		journal.Load ();
		hangar.Init (shipsSaves, cometUnlocks, journal);
		hangar.Show ();
		if (newGame) {
			hangar.ShowStartMessage ();
		}
		hangar.lastBoughtPowerup = null;
	}

	void HandleStartTheGame (MSpaceshipData shipData) {
		hangar.Hide ();
		ToggleUICamera (false);
		gameObjects.ForEach (h => h.SetActive (true));
		main.StartTheGame (shipData, GetActiveComets(), DetermineLvel(), hangar.lastBoughtPowerup);
		hangar.lastBoughtPowerup = null;
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
		Debug.LogWarning ("GameOver");
		StartCoroutine (FinishGameIn (3f, false));
	}

	IEnumerator FinishGameIn(float seconds, bool success) {
		yield return new WaitForSeconds (seconds);

		ToggleUICamera (true);
		gameObjects.ForEach (h => h.SetActive (false));

		yield return new WaitForEndOfFrame ();
		yield return new WaitForEndOfFrame ();
	
		main.Clear ();
		hangar.Show ();
		if (currentLevelIndex >= 0) {
			if (success) {
				SaveLevelPassed (currentLevelIndex);
			}
			var level = MLevelsResources.Instance.levels [currentLevelIndex];
			if (level != null) {
				hangar.ShowMessage (level.journal);
				if (success) {
					hangar.ShowMessage (level.journalFinish);
				}
			}
		}
	}

	void SaveLevelPassed(int levelIndx){
		int lastLevelPassed = PlayerPrefs.GetInt (GetLevelSaveKey (currentSlot), 0);
		if (levelIndx + 1 > lastLevelPassed) {
			Debug.LogError ("level passed: " + (levelIndx + 1));
			PlayerPrefs.SetInt (GetLevelSaveKey (currentSlot), levelIndx + 1);
		}
	}

	ILevelSpawner DetermineLvel(){
		currentLevelIndex = -1;
		ILevelSpawner spawner;
		int level = int.Parse (levelInput.text);
		int waveNum = int.Parse (waveInput.text);
		if (level < 0) {
			spawner = new EmptyTestSceneSpawner ();
		} else {
			currentLevelIndex = level;
			var lvl = MLevelsResources.Instance.levels [level];
			spawner = lvl.GetLevel();
			//testing purposes
			if (waveNum != 0) {
				(spawner as LevelSpawner).ForceWaveNum(waveNum);
			}
		}
		return spawner;
	}

	void OnApplicationPause(){
		PlayerPrefs.Save ();
	}

	void OnApplicationQuit(){
		PlayerPrefs.Save ();
	}
}
