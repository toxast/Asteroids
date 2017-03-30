using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class UIShipsScroll : MonoBehaviour 
{
	[SerializeField] BuyShipElem prefab;
	[SerializeField] RectTransform elementsHolder;

	public BuyShipElem selected { get; private set;}
	List<BuyShipElem> list = new List<BuyShipElem>();



	void Awake(){
//		LoadShips ();
	}

	public Action<BuyShipElem> OnCick;

	public void Show(List<ShipUpgrades> elemsData) {
//		Clear ();
//		bool lastShipFullUpgraded = false;
//		for (int i = 0; i < elemsData.Count; i++) {
//			var upgradeList = elemsData [i].ships;
//			var lastBoughtUpgrade = upgradeList.FindLast(s => unlockedShips.Contains(s.id));
//
//			if (lastBoughtUpgrade != null) {
//				bool canBeUpgraded = upgradeList.Last () != lastBoughtUpgrade;
//				BuyShipElem.State state = canBeUpgraded ? BuyShipElem.State.CanBeUpgraded : BuyShipElem.State.Max;
//				AddShipToTheList (lastBoughtUpgrade, state);
//				lastShipFullUpgraded = !canBeUpgraded;
//			} else {
//				if (lastShipFullUpgraded) {
//					//show awaliable
//					AddShipToTheList (upgradeList[0],  BuyShipElem.State.CanUnlock);
//				} else {
//					//display lock
//					AddShipToTheList (upgradeList[0],  BuyShipElem.State.CanUnlock);
//				}
//
//				lastShipFullUpgraded = false;
//			}
//		}
	}

	BuyShipElem AddShipToTheList(MSpaceshipData data, BuyShipElem.State state){
		var shipElem = Instantiate (prefab) as BuyShipElem;
		shipElem.transform.SetParent (elementsHolder, false);
		shipElem.Refresh (data, state); 
		shipElem.OnClick += () => OnShipElemClicked(shipElem);
		list.Add (shipElem);
		return shipElem;
	}

	void OnShipElemClicked(BuyShipElem elem){
		OnCick (elem);
	}

	public void Clear() {
		selected = null;
		list.ForEach (item => Destroy (item.gameObject));
		list.Clear ();
	}

	public void Select(BuyShipElem data)
	{
		if (selected != data && selected != null) {
			selected.Unselect ();
		}
		selected = data;
		if (selected != null) {
			selected.Select ();
		}
	}

	void UnlockShip(int id){
//		unlockedShips.Add (id);
//		SaveShips ();
	}


}

public class ShipsSave {

	const string shipsSaveString = "unlockedShips";
	public HashSet<int> unlockedShips = new HashSet<int>{1};

	public void SaveShips() {
		string saveString = string.Empty;
		var list = unlockedShips.ToList ();
		for (int i = 0; i < list.Count; i++) {
			saveString += list[i].ToString() + " ";
		}
		PlayerPrefs.SetString (shipsSaveString, saveString);
	}

	public void UnlockShip(int id){
		unlockedShips.Add (id);
		SaveShips ();
	}

	public void LoadShips() {
		string loadedString = PlayerPrefs.GetString (shipsSaveString, "");
		var parts = loadedString.Split (' ');
		for (int i = 0; i < parts.Count(); i++) {
			int res;
			if(System.Int32.TryParse(parts[i], out res)){
				unlockedShips.Add (res);
			}
		}
	}
}
  