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

	public void Show() {
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
			var lastBoughtUpgradeIndex = upgradeList.FindLastIndex(s => cometUnlocks.Contains(s.id));
			if (lastBoughtUpgradeIndex >= 0) {
				lastBoughtUpgrade = upgradeList [lastBoughtUpgradeIndex];
			}
			PowerupUpgradeData pdata = new PowerupUpgradeData ();
			PowerupUI.State state;
			if (lastBoughtUpgrade != null) {
				bool canBeUpgraded = (lastBoughtUpgradeIndex + 1) < upgradeList.Count;
				state = canBeUpgraded ? PowerupUI.State.CanBeUpgraded : PowerupUI.State.Max;
				pdata.current = lastBoughtUpgrade;
				if (canBeUpgraded) {
					pdata.next = upgradeList [lastBoughtUpgradeIndex + 1];
				}
			} else {
				if (previousPowerupUnlocked) {
					//show awaliable
					pdata.current = upgradeList[0];
					state = PowerupUI.State.CanUnlock;
				} else {
					//display lock
					pdata.current = upgradeList[0];
					state = PowerupUI.State.Locked;
				}
			}
			list[i].Refresh(pdata, state);
			previousPowerupUnlocked = lastBoughtUpgrade != null;
		}
	}

	void OnElemBuy(PowerupUI elem) {
		MCometData toUnlock = null;
		if (elem.state == PowerupUI.State.CanBeUpgraded) {
			toUnlock = elem.data.next;
		} else if(elem.state == PowerupUI.State.CanUnlock) {
			toUnlock = elem.data.current;
		}

		if (toUnlock != null) {
			int price = toUnlock.price;
			if (GameResources.SpendMoney (price)) {
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
}

