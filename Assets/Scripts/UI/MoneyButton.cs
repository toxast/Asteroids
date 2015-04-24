using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class MoneyButton : MonoBehaviour 
{
	[SerializeField] Text price;
	public event Action<MoneyButton> click;

	public void SetPrice(int m)
	{
		price.text = m.ToString ();
	}

	public void OnClickMe()
	{
		if (click != null)click (this);
	}

	

}
