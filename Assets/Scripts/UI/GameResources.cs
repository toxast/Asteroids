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
	static string moneySaveKey = "";


	public static int money {
		get;
		private set;
	}

	public static void LoadMoney(string saveKey){
		moneySaveKey = saveKey;
		var loaded = PlayerPrefs.GetInt (moneySaveKey, 0);
		AddMoney (loaded);
		Logger.Log ("money loaded: " + money);
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
