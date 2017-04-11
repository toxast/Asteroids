using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMainMenu : MonoBehaviour {
	[SerializeField] Canvas canvas;

	[SerializeField] GameObject containerMenu;
	[SerializeField] Button continueBtn;
	[SerializeField] Button startBtn;

	[SerializeField] GameObject containerSlots;
	[SerializeField] UISaveSlot slot1;
	[SerializeField] UISaveSlot slot2;
	[SerializeField] UISaveSlot slot3;

	[NonSerialized] List<UISaveSlot> slotElems = new List<UISaveSlot>();

	bool Visible {
		get{return canvas.enabled;}
		set{canvas.enabled = value;}
	}
	const string lastPlayedSlotSaveKey = "lastSlot";

	public Action<int> OnLoadGame;
	int lastPlayedSlot;

	void Awake(){
		slotElems.Add (slot1);
		slotElems.Add (slot2);
		slotElems.Add (slot3);
		foreach (var slot in slotElems) {
			var localSlot = slot;
			localSlot.OnClick += () => HandleSlotClicked(localSlot);
			localSlot.OnDelete += () => HandleSlotDelete(localSlot);
		}
		continueBtn.onClick.AddListener (HandleContinue);
		startBtn.onClick.AddListener (HandleStart);
	}

	public void Show() {
		ToggleView (true);
		lastPlayedSlot = LoadLastPlayedSlot();
		continueBtn.gameObject.SetActive (lastPlayedSlot > 0);
		Visible = true;
	}

	int LoadLastPlayedSlot(){
		int saved = PlayerPrefs.GetInt (lastPlayedSlotSaveKey, 0);
		if (saved > 0 && PlayerPrefs.GetInt (Game.GetGameStartedSaveKey (saved.ToString()), 0) == 1) {
			return saved;
		} else {
			return 0;
		}
	}

	private void ToggleView(bool main){
		containerMenu.SetActive (main);
		containerSlots.SetActive (!main);
	}

	void HandleContinue(){
		FireLoadGame (lastPlayedSlot);
	}

	void HandleStart() {
		ToggleView (false);
		RefreshSlotViews ();
	}

	void HandleSlotDelete(UISaveSlot slotElem){
		Game.DeleteSaves (slotElem.id.ToString ());
		RefreshSlotViews ();
	}

	void HandleSlotClicked(UISaveSlot slotElem){
		FireLoadGame (slotElem.id);
	}

	private void FireLoadGame(int id){
		PlayerPrefs.SetInt (lastPlayedSlotSaveKey, id);
		OnLoadGame (id);
		Visible = false;
	}

	void RefreshSlotViews(){
		for (int slotId = 1; slotId <= 3; slotId++) {
			string slot = slotId.ToString ();
			var playedSlot = PlayerPrefs.GetInt (Game.GetGameStartedSaveKey (slot), 0) == 1;
			var slotUI = slotElems [slotId - 1];
			if (playedSlot) {
				int lastLevel = PlayerPrefs.GetInt (Game.GetLevelSaveKey (slot), 0);
				int money = PlayerPrefs.GetInt (Game.GetMoneySaveKey (slot), 0);
				slotUI.Refresh (slotId, lastLevel + 1, money);
			} else {
				slotUI.Refresh (slotId);
			}
		}
	}
}

