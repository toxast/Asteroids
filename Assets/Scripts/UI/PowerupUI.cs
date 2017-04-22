using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class PowerupUI : MonoBehaviour 
{
	[SerializeField] Text label;
	[SerializeField] GameObject lockedContainer;
	[SerializeField] GameObject normalContainer;
	[SerializeField] GameObject lockedByItemingicator;
	[SerializeField] PriceButton priceButton;

	public Action OnClick;
	public Action OnAction;

	public PowerupUpgradeData data{ get; private set;}

	void Awake(){
		priceButton.clickCallback += () => OnAction();
	}

	public void Refresh(PowerupUpgradeData data)
	{
		this.data = data;
		bool locked = !data.previousPowerupBought;
		normalContainer.SetActive (!locked);
		lockedContainer.SetActive (locked);
		if (!locked) {
			var current = data.current;
			var next = data.next;
			var main = current != null ? current : next;
			label.text = main.name;

			if (!data.lockedByItem) {
				lockedByItemingicator.SetActive (false);
				priceButton.gameObject.SetActive (next != null);
				priceButton.SetState (current == null);
				if (next != null) {
					priceButton.Refresh (next.price, false);
				}
			} else {
				priceButton.gameObject.SetActive (false);
				lockedByItemingicator.SetActive (true);
			}
		}
	}

	public void Select()
	{
	}

	public void Unselect()
	{
	}
}
