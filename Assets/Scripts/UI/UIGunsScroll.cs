using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class UIGunsScroll : MonoBehaviour 
{
	[SerializeField] BuyShipElem prefab;
	[SerializeField] RectTransform elementsHolder;
	
	BuyShipElem selected = null;
	List<BuyShipElem> list = new List<BuyShipElem>();
	
	public void Show(List<GUIHangar.GunIndexWarpper> elemsData, Action<int, GUIHangar.GunIndexWarpper> onClick)
	{
		Clear ();
		
		for (int i = 0; i < elemsData.Count; i++) 
		{
			int index = i;
			var data = elemsData [index];
			
			var shipElem = Instantiate(prefab) as BuyShipElem;
			shipElem.transform.SetParent(elementsHolder, false);
			shipElem.Init(data.iname, data.iprice); 
			shipElem.AddListener(() => onClick(index, data));
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
