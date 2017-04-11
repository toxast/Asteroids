using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class PowerupUI : MonoBehaviour 
{
	[SerializeField] Text label;
	[SerializeField] GameObject lockedContainer;
	[SerializeField] GameObject normalContainer;
	[SerializeField] PriceButton priceButton;

	public Action OnClick;
	public Action OnAction;

	public PowerupUpgradeData data{ get; private set;}
	public State state{ get; private set;}
	public enum State {
		CanBeUpgraded = 1,
		Max,
		CanUnlock,
		Locked,
	}

	void Awake(){
		priceButton.clickCallback += () => OnAction();
	}

	public void Refresh(PowerupUpgradeData data, State state)
	{
		this.data = data;
		this.state = state;

		bool locked = state == State.Locked;
		normalContainer.SetActive (!locked);
		lockedContainer.SetActive (locked);
		label.text = data.current.name;

		//label.gameObject.SetActive (state != State.CanUnlock && state != State.Locked);

		if (state == State.CanBeUpgraded) {
			priceButton.gameObject.SetActive (true);
			priceButton.Refresh (data.next.price, false);
		} else if(state == State.CanUnlock){
			priceButton.gameObject.SetActive (true);
			priceButton.Refresh (data.current.price, true);
		} else {
			priceButton.gameObject.SetActive (false);
		}
	}

	public void Select()
	{
	}

	public void Unselect()
	{
	}
}
