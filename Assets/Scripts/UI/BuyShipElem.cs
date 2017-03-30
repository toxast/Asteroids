using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class BuyShipElem : MonoBehaviour 
{
	[SerializeField] Text label;
	[SerializeField] Button clickArea;

	[SerializeField] GameObject canBeUpgradedContainer;
	[SerializeField] Text upgradeText;

	[SerializeField] GameObject maxContainer;
	[SerializeField] Text maxText;

	[SerializeField] GameObject unlockContainer;
	[SerializeField] Text unlockText;

	[SerializeField] GameObject lockedContainer;


	[SerializeField] Image selectElem;

	public Action OnClick;

	public MSpaceshipData data{ get; private set;}
	public State state{ get; private set;}
	public enum State
	{
		CanBeUpgraded = 1,
		Max,
		CanUnlock,
		Locked,
	}

	void Awake(){
		clickArea.onClick.AddListener (() => OnClick());
	}

	public void Refresh(MSpaceshipData data, State state)
	{
		this.data = data;
		this.state = state;

		canBeUpgradedContainer.SetActive (state == State.CanBeUpgraded);
		maxContainer.SetActive (state == State.Max);
		unlockContainer.SetActive (state == State.CanUnlock);
		lockedContainer.SetActive (state == State.Locked);

		switch (state) {
		case State.CanBeUpgraded:
			//text and upgrade arrow
			upgradeText.text = data.name;
			break;
		case State.Max:
			maxText.text = data.name;
			break;
		case State.CanUnlock:
			unlockText.text = "Unlock " + data.price;
			break;
		case State.Locked:
			break;
		}

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
