using UnityEngine;
using System;
using System.Collections;

public static class GameResources 
{
	static public event Action<float> healthChanged;
	static public void SetHealth(float h)
	{
		if(healthChanged != null)
			healthChanged (h);
	}


	static public event Action<float> shieldsChanged;
	static public void SetShields(float s)
	{
		if(shieldsChanged != null) 
			shieldsChanged (s);
	}


	static public event Action<int> moneyChanged = delegate {};
	const string moneySaveKey = "essence";
	public static void LoadMoney() {
		int money = PlayerPrefs.GetInt (moneySaveKey, 0);
		AddMoney (money);
	}

	public static int money {
		get;
		private set;
	}

	public static void AddMoney(int amount)	{
		money += amount;
		PlayerPrefs.SetInt (moneySaveKey, money);
		moneyChanged (money);
	}

	public static bool SpendMoney(int amount) {
		if (CanSpend(amount)) {
			money -= amount;
			PlayerPrefs.SetInt (moneySaveKey, money);
			moneyChanged (money);
			return true;
		} else {
			return false;
		}
	}

	public static bool CanSpend(int amount) {
		return money >= amount;
	}
}
