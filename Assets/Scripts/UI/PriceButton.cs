using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PriceButton : ui5.TextButtonToggle5 {
	void OnEnable(){
		GameResources.moneyChanged += OnMoneyChange;
	}

	void OnDisable(){
		GameResources.moneyChanged -= OnMoneyChange;
	}

	int money = 0;
	public void Refresh(int money, bool isUnlock) {
		this.money = money;
		Title = money.ToString ();
		SetState (isUnlock);
		RefreshInteractable ();
	}

	void OnMoneyChange(int m) {
		RefreshInteractable ();
	}

	private void RefreshInteractable() {
		SetInteractable(GameResources.CanSpend(money));
	}

}
