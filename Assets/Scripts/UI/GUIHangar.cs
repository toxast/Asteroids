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
	[SerializeField] UIGunsScroll gunsScroll;
	[SerializeField] Button startButton;

	FullSpaceShipSetupData currentShipData = null;
	public event Action<FullSpaceShipSetupData> startTheGame;

	void Awake()
	{
		startButton.onClick.AddListener (() => StartGame());
	}

	void Start ()
	{
		shipsScroll.Show (SpaceshipsResources.Instance.spaceships, Create);
	}

	void StartGame()
	{
		if(currentShipData != null)
		{
			startTheGame(currentShipData);
			uiShip.Clear();
		}
	}

	private void Create(int shipIndx, FullSpaceShipSetupData data)
	{
		shipsScroll.Select (shipIndx);
		currentShipData = data.Clone();
		uiShip.Create (shipIndx, data, SelectCannonClicked);

		gunsScroll.Clear ();
	}

	private void SelectCannonClicked(int cannonNum)
	{
		var guns = GetAvaliableGuns (currentShipData, cannonNum);
		Action<int, GUIHangar.GunIndexWarpper> act = (indx, gun) =>
		{
			gunsScroll.Select(indx);
			currentShipData.guns [cannonNum].type = gun.itype;
			currentShipData.guns [cannonNum].index = gun.index;
		};

		gunsScroll.Show (guns, act);

		var currentGun = currentShipData.guns [cannonNum];
		if(currentGun.type != GunSetupData.eGuns.None)
		{
			var i = guns.FindIndex( g => g.gun.itype == currentGun.type && g.index == currentGun.index);

			if(i >= 0)
				gunsScroll.Select(i);
		}
	}

	private List<GunIndexWarpper> GetAvaliableGuns(FullSpaceShipSetupData spaceship, int cannonNum)
	{
		//var cannon = spaceship.guns [cannonNum];
		List<GunIndexWarpper> guns = new List<GunIndexWarpper> ();
		for (int i = 0; i < GunsResources.Instance.guns.Count; i++) {
			guns.Add(new GunIndexWarpper{gun = GunsResources.Instance.guns[i], index = i});
		}
		for (int i = 0; i < GunsResources.Instance.rocketLaunchers.Count; i++) {
			guns.Add(new GunIndexWarpper{gun = GunsResources.Instance.rocketLaunchers[i], index = i});
		}
		for (int i = 0; i < GunsResources.Instance.lazerGuns.Count; i++) {
			guns.Add(new GunIndexWarpper{gun = GunsResources.Instance.lazerGuns[i], index = i});
		}
		for (int i = 0; i < GunsResources.Instance.spawnerGuns.Count; i++) {
			guns.Add(new GunIndexWarpper{gun = GunsResources.Instance.spawnerGuns[i], index = i});
		}
		return guns;
	}
	
	public class GunIndexWarpper: IGun
	{
		public IGun gun;
		public int index;

		public string iname{ get {return gun.iname;}}
		public int iprice{ get {return gun.iprice;}}
		public GunSetupData.eGuns itype{ get {return gun.itype;}}
	}
}
