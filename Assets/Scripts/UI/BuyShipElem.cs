using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class BuyShipElem : MonoBehaviour 
{
	[SerializeField] Text label;
	[SerializeField] MoneyButton btn;

	public event Action<BuyShipElem> click;

	void Awake()
	{
		btn.click += HandleClick;
	}

	public void Init(string text, int price)
	{
		label.text = text;
		btn.SetPrice (price);
	}

	void HandleClick (MoneyButton obj)
	{
		if (click != null)
			click (this);
	}


}
