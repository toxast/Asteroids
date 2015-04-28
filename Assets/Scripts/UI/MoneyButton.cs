using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class MoneyButton : MonoBehaviour 
{
	[SerializeField] Text price;
	[SerializeField] Button clickArea;

	public void SetPrice(int m)
	{
		price.text = m.ToString ();
	}

	public void AddListener(Action act)
	{
		clickArea.onClick.AddListener (() => act());
	}

}
