﻿using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GUIHangar : MonoBehaviour 
{
	[SerializeField] Canvas canvas;
	[SerializeField] Text moneyText;
	[SerializeField] UIPowerupsScroll powerupsScroll;
	[SerializeField] Image powerupsLock;
	[SerializeField] UIShip uiShip;
	[SerializeField] MessageTyper messageText;
	[SerializeField] LevelSelectUI levelSelect;

	[SerializeField] PriceButton unlockButton;

	const int UNLOCK_SHIP_ID_POWERUPS = 2;

	public event Action<MSpaceshipData, int> startTheGame;

	[NonSerialized] public Queue<MCometData> lastBoughtPowerups = new Queue<MCometData>();
	ShipUpgradeData currentShipData;
	IntHashSave shipsSaves;
	IntHashSave journalSaves;

	bool Visible{
		get{return canvas.enabled;}
		set{canvas.enabled = value;}
	}

	public bool IsUnlocked(int shipId){
		if (shipsSaves == null) {
			return false;
		}
		return shipsSaves.Contains (shipId);
	}

	void Awake() {
		levelSelect.OnStart += LevelSelect_OnStart;
		GameResources.moneyChanged += GameResources_moneyChanged;
		unlockButton.clickCallback += UnlockShip;
		powerupsScroll.OnBought += OnPowerupBought;
	}

	void LevelSelect_OnStart (int levelIndx) {
		startTheGame(currentShipData.current, levelIndx);
	}

	public void AnimatePowerupsUnlock(){
		powerupsLock.gameObject.SetActive (false);
	}

	public void Init(IntHashSave shipsSaves, IntHashSave cometUnlocks, IntHashSave journalSaves){
		this.shipsSaves = shipsSaves;
		this.journalSaves = journalSaves;
		powerupsScroll.Init (cometUnlocks);
	}

	public void Show (int avaliableLevelIndex) {
		Visible = true;
		ViewLastBoughtShip ();
		powerupsScroll.Refresh ();
		powerupsLock.gameObject.SetActive (currentShipData.current.id < UNLOCK_SHIP_ID_POWERUPS);
		levelSelect.Refresh (avaliableLevelIndex, MLevelsResources.Instance.levels.Count);
	}

	public void Hide(){
		Clear ();
		Visible = false;
	}

	void Clear() {
		uiShip.Clear();
		powerupsScroll.Clear ();
		ClearMessage ();
	}

	class ShipUpgradeData{
		public MSpaceshipData current;
		public MSpaceshipData next;
		public bool isNextShipType;
	}
		
	ShipUpgradeData FindLastBounghtShip() {
		var shipUpgrades = ResourceSingleton<MSpaceShipResources>.Instance.userSpaceships;
		for (int i = shipUpgrades.Count - 1; i >= 0; i--) {
			var upgrades = shipUpgrades [i].ships;
			var indx = upgrades.FindLastIndex (sh => shipsSaves.Contains (sh.id));
			if (indx >= 0) {
				MSpaceshipData lastBoughtShip = upgrades [indx];
				MSpaceshipData nextShip = null;
				bool isNextShipType;
				if (indx + 1 < upgrades.Count) {
					isNextShipType = false;
					nextShip = upgrades [indx + 1];
				} else {
					isNextShipType = true;
					if (i + 1 < shipUpgrades.Count) {
						nextShip = shipUpgrades [i + 1].ships [0];
					}
				}
				return new ShipUpgradeData{ current = lastBoughtShip, next = nextShip, isNextShipType = isNextShipType };
			}
		}
		return null;
	}

	void GameResources_moneyChanged (int money) {
		moneyText.text = money.ToString ();
	}

	void OnPowerupBought(MCometData comet){
		if (!comet.dropFromEnemies) {
			lastBoughtPowerups.Enqueue(comet);
		}
		ShowMessage (comet.journal);
	}

	void UnlockShip() {
		var unlockShip = currentShipData.next;
		if (unlockShip == null) {
			return;
		}
		if (GameResources.SpendMoney (unlockShip.price)) {
			Logger.Log ("UNLOCK SHIP" + unlockShip.name + " for " + unlockShip.price);
			shipsSaves.Add (unlockShip.id);
			ViewLastBoughtShip ();
			if (unlockShip.journal != null) {
				ShowMessage (unlockShip.journal);
			}
			if (unlockShip.id == UNLOCK_SHIP_ID_POWERUPS) {
				AnimatePowerupsUnlock ();
			}
			powerupsScroll.Refresh ();
		}
	}

	void ViewLastBoughtShip() {
		currentShipData = FindLastBounghtShip ();
		uiShip.Create (currentShipData.current);
		unlockButton.gameObject.SetActive (currentShipData.next != null);
		if (currentShipData.next != null) {
			unlockButton.Refresh (currentShipData.next.price, currentShipData.isNextShipType);
		}
	}

	public void ShowStartMessage() {
		ShowMessage (ResourceSingleton<MSpaceShipResources>.Instance.userSpaceships[0].ships[0].journal);
	}

	public void ShowMessage(MJournalLog log) {
		if (log != null && !journalSaves.Contains(log.id)) {
			journalSaves.Add (log.id);
			messageText.Show (log.text);
		}
	}

	public void ClearMessage() {
		messageText.Clear();
	}

	private void HandleClick(PowerupUI elem) {

	}
}
