using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class GUIHangar : MonoBehaviour 
{
	[SerializeField] UIShipsScroll shipsScroll;
	[SerializeField] UIShip uiShip;
	[SerializeField] UIGunsScroll gunsScroll;

	FullSpaceShipSetupData currentShipData = null;

	void Start ()
	{
		shipsScroll.Show (SpaceshipsResources.Instance.spaceships, Create);
	}

	private void Create(int shipIndx, FullSpaceShipSetupData data)
	{
		shipsScroll.Select (shipIndx);
		currentShipData = data.Clone();
		uiShip.Create (shipIndx, data, SelectCannonClicked);
	}

	private void SelectCannonClicked(int cannonNum)
	{
		var guns = GetAvaliableGuns (currentShipData, cannonNum);
		Action<int, GUIHangar.GunIndexWarpper> act = (indx, gun) =>
		{
			gunsScroll.Select(indx);
//			GunSelected(cannonNum, gun.itype, gun.index);
			currentShipData.guns [cannonNum].type = gun.itype;
			currentShipData.guns [cannonNum].index = gun.index;
		};

		gunsScroll.Show (guns, act);
	}

	private List<GunIndexWarpper> GetAvaliableGuns(FullSpaceShipSetupData spaceship, int cannonNum)
	{
		var cannon = spaceship.guns [cannonNum];
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
	
//	private void GunSelected(int cannonNum, GunSetupData.eGuns gunType, int indx)
//	{
//		Debug.LogWarning ("GunSelected " + cannonNum + " " + gunType + " " + indx);
//		currentShipData.guns [cannonNum].type = gunType;
//		currentShipData.guns [cannonNum].index = indx;
//	}

	public class GunIndexWarpper: IGun
	{
		public IGun gun;
		public int index;

		public string iname{ get {return gun.iname;}}
		public int iprice{ get {return gun.iprice;}}
		public GunSetupData.eGuns itype{ get {return gun.itype;}}
	}
}
