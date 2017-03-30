using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GUIHangar : MonoBehaviour 
{
	[SerializeField] Text moneyText;
	//[SerializeField] UIShipsScroll shipsScroll;
	[SerializeField] UIShip uiShip;
	[SerializeField] Text messageText;

	[SerializeField] ui5.TextButton5 unlockButton;
	[SerializeField] SpriteToggle5 toggleUnlock;

	[SerializeField] Button startButton;

	public event Action<MSpaceshipData> startTheGame;

	ShipsSave shipsSaves = new ShipsSave();
	ShipUpgradeData currentShipData;

	void Awake() {
		shipsSaves.LoadShips ();
		startButton.onClick.AddListener (FireStartGame);
		GameResources.moneyChanged += GameResources_moneyChanged;
		unlockButton.onClick.AddListener (UnlockShip);
	}

	void Start () {
		ViewLastBoughtShip ();
	}

	void FireStartGame() {
		uiShip.Clear();
		ClearMessage ();
		startTheGame(currentShipData.ship);
	}

	class ShipUpgradeData{
		public MSpaceshipData ship;
		public MSpaceshipData next;
		public bool isNextShipType;
	}

	ShipUpgradeData FindLastBounghtShip() {
		var shipUpgrades = ResourceSingleton<MSpaceShipResources>.Instance.userSpaceships;
		for (int i = shipUpgrades.Count - 1; i >= 0; i--) {
			var upgrades = shipUpgrades [i].ships;
			var indx = upgrades.FindLastIndex (sh => shipsSaves.unlockedShips.Contains (sh.id));
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
				return new ShipUpgradeData{ ship = lastBoughtShip, next = nextShip, isNextShipType = isNextShipType };
			}
		}
		return null;
	}

	void GameResources_moneyChanged (int money) {
		moneyText.text = money.ToString ();
	}

	void UnlockShip() {
		if (GameResources.SpendMoney (currentShipData.next.price)) {
			shipsSaves.UnlockShip (currentShipData.next.id);
			ViewLastBoughtShip ();
			ShowMessage ("hey cool");
		}
	}

	void ShowMessage(string msg) {
		messageText.text = msg;
	}

	void ClearMessage() {
		messageText.text = string.Empty;
	}

	void ViewLastBoughtShip() {
		currentShipData = FindLastBounghtShip ();
		uiShip.Create (currentShipData.ship);
		unlockButton.gameObject.SetActive (currentShipData.next != null);
		if (currentShipData.next != null) {
			unlockButton.text = currentShipData.next.price.ToString();
		}
		toggleUnlock.SetState (currentShipData.isNextShipType);
	}

	private void ShowShip(MSpaceshipData current, MSpaceshipData next, bool isUnlock){
		uiShip.Create (current);
		toggleUnlock.SetState (isUnlock);
	}

	private void HandleClick(BuyShipElem elem) {
//		switch (elem.state) {
//		case BuyShipElem.State.CanBeUpgraded:
//			shipsScroll.Select (elem);
//			uiShip.Create (elem.data);
//			break;
//		case BuyShipElem.State.Max:
//			shipsScroll.Select (elem);
//			uiShip.Create (elem.data);
//			break;
//		case BuyShipElem.State.CanUnlock:
//			//unlock
//			break;
//		case BuyShipElem.State.Locked:
//			//nothing
//			break;
//		}
	}
}
