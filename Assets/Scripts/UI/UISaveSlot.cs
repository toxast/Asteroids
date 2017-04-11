using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISaveSlot: MonoBehaviour {
	[SerializeField] Text nameText;
	[SerializeField] Button clickArea;
	[SerializeField] Button deleteBtn;

	[SerializeField] GameObject playInfoContainer;
	[SerializeField] Text levelText;
	[SerializeField] Text moneyText;

	public int id{ get; private set;}
	public bool played { get; private set;}

	public Action OnClick;
	public Action OnDelete;

	void Awake(){
		clickArea.onClick.AddListener (() => OnClick ());
		deleteBtn.onClick.AddListener (() => OnDelete ());
	}

	public void Refresh(int id)	{
		this.id = id;
		played = false;
		nameText.text = "New Game";
		deleteBtn.gameObject.SetActive (false);
		playInfoContainer.SetActive (false);
	}

	public void Refresh(int id, int unlockedLevel, int money)	{
		this.id = id;
		played = true;
		nameText.text = "Slot " + id;
		levelText.text = "Level " + unlockedLevel.ToString ();
		moneyText.text = money.ToString ();
		deleteBtn.gameObject.SetActive (true);
		playInfoContainer.SetActive (true);
	}

}
