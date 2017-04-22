using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class UIPowerupsScroll : MonoBehaviour {
	[SerializeField] PowerupUI prefab;
	[SerializeField] RectTransform elementsHolder;

	public Action<MCometData> OnBought;
	//public PowerupUI selected { get; private set;}
	List<PowerupUI> list = new List<PowerupUI>();
	IntHashSave cometUnlocks;

	public void Init(IntHashSave cometUnlocks){
		this.cometUnlocks = cometUnlocks;
	}

	public void Refresh() {
		Clear ();
		var elemsData = MPowerUpResources.Instance.powerups;
		for (int i = 0; i < elemsData.Count; i++) {
			var elem = Instantiate (prefab) as PowerupUI;
			elem.transform.SetParent (elementsHolder, false);
			elem.OnAction += () => OnElemBuy(elem);
			list.Add (elem);
		}
		UpdateListView ();
	}

	public void Clear() {
		list.ForEach (item => Destroy (item.gameObject));
		list.Clear ();
	}

	private void UpdateListView() {
		var elemsData = MPowerUpResources.Instance.powerups;
		bool previousPowerupUnlocked = true;
		for (int i = 0; i < list.Count; i++) {
			var upgradeList = elemsData [i].comets;
			MCometData lastBoughtUpgrade = null;
			PowerupUpgradeData pdata = new PowerupUpgradeData ();
			var lastBoughtUpgradeIndex = upgradeList.FindLastIndex(s => cometUnlocks.Contains(s.id));
			if (lastBoughtUpgradeIndex >= 0) {
				lastBoughtUpgrade = upgradeList [lastBoughtUpgradeIndex];
			}
			pdata.previousPowerupBought = previousPowerupUnlocked;
			if (lastBoughtUpgrade != null) {
				pdata.current = lastBoughtUpgrade;
				if (lastBoughtUpgradeIndex + 1 < upgradeList.Count) {
					pdata.next = upgradeList [lastBoughtUpgradeIndex + 1];
				}
			} else {
				pdata.next = upgradeList[0];
			}
			pdata.lockedByItem = false;
			if (pdata.next != null) {
				var restiction = pdata.next.shipRestricltion;
				pdata.lockedByItem = restiction != null && !Singleton<GUIHangar>.inst.IsUnlocked (restiction.id);
			}
			list[i].Refresh(pdata);
			previousPowerupUnlocked = lastBoughtUpgrade != null;
		}
	}

	void OnElemBuy(PowerupUI elem) {
		MCometData toUnlock = elem.data.next;
		if (toUnlock != null) {
			int price = toUnlock.price;
			if (GameResources.SpendMoney (price)) {
				Logger.Log ("UNLOCK POWERUP" + toUnlock.name + " for " + price);
				cometUnlocks.Add (toUnlock.id);
			}
			UpdateListView ();
			OnBought (toUnlock);
		}
	}
}
  
public class PowerupUpgradeData{
	public MCometData current;
	public MCometData next;
	public bool previousPowerupBought;
	public bool lockedByItem;
}

