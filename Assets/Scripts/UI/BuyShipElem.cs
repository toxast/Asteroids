using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class BuyShipElem : MonoBehaviour 
{
	[SerializeField] Text label;
	[SerializeField] MoneyButton btn;
	[SerializeField] Image selectElem;

	public void AddListener(Action act)
	{
		btn.AddListener (act);
	}

	public void Init(string text, int price)
	{
		label.text = text;
		btn.SetPrice (price);
	}

	public void Select()
	{
		selectElem.enabled = true;
	}

	public void Unselect()
	{
		selectElem.enabled = false;
	}
}
