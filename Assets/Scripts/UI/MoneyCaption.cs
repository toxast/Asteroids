using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MoneyCaption : MonoBehaviour
{
	[SerializeField] Text label;

	void Awake()
	{
		GameResources.moneyChanged += (m) =>
		{
			label.text = m.ToString();
		};
	}
}
