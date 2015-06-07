﻿using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class UIShipsScroll : MonoBehaviour 
{
	[SerializeField] BuyShipElem prefab;
	[SerializeField] RectTransform elementsHolder;

	BuyShipElem selected = null;
	List<BuyShipElem> list = new List<BuyShipElem>();

	public void Show(List<FullSpaceShipSetupData> elemsData, Action<int, int, FullSpaceShipSetupData> onClick)
	{
		Clear ();

		for (int i = 0; i < elemsData.Count; i++) 
		{
			int index = i;
			var data = elemsData [index];
			if(data.price <= 0)
				continue;

			var shipElem = Instantiate(prefab) as BuyShipElem;
			shipElem.transform.SetParent(elementsHolder, false);
			shipElem.Init(data.name, data.price); 
			int indxInList = list.Count;
			shipElem.AddListener(() => onClick(indxInList, index, data));
			list.Add(shipElem);
		}
	}

	public void Clear()
	{
		selected = null;
		list.ForEach (item => Destroy (item.gameObject));
		list.Clear ();
	}

	public void Select(int i)
	{
		if (selected != null)
			selected.Unselect ();

		selected = list [i];
		selected.Select();
	}
}
