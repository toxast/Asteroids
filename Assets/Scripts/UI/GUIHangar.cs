using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GUIHangar : MonoBehaviour 
{
	[SerializeField] UIShipsScroll shipsScroll;
	[SerializeField] UIShip uiShip;
	[SerializeField] Button startButton;

	MSpaceshipData currentShipData = null;
	public event Action<MSpaceshipData> startTheGame;

	void Awake() {
		startButton.onClick.AddListener (() => StartGame());
	}

	void Start () {
		shipsScroll.Show (MSpaceShipResources.Instance.userSpaceships, onClick: Create);
	}

	void StartGame() {
		if(currentShipData != null) {
			startTheGame(currentShipData);
			uiShip.Clear();
		}
	}

	private void Create(int shipIndx, MSpaceshipData data) {
		shipsScroll.Select (shipIndx);
		currentShipData = data;
		uiShip.Create (data);
	}
}
