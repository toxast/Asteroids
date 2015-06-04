using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class UIShip : MonoBehaviour 
{
	[SerializeField] Camera cam;
	[SerializeField] Button canonImgPrefab;
	[SerializeField] Transform cannonsHolder;

	SpaceShip spaceship;
	List<Button> cannons = new List<Button> ();

	public void Create(int shipIndx, FullSpaceShipSetupData data, Action<int> cannonClicked)
	{
		Clear ();
		
		spaceship = PolygonCreator.CreatePolygonGOByMassCenter<SpaceShip> (data.verts, data.color);
		spaceship.InitPolygonGameObject (data.physical);
		spaceship.SetThrusters (data.thrusters);
		spaceship.ShowFullThruster ();
		spaceship.cacheTransform.position = new Vector3(0, 0, 70);
		Debug.LogWarning ("created " + shipIndx + " health:" + spaceship.fullHealth + " area:" + spaceship.polygon.area + " R:" + spaceship.polygon.R);
		int gunIndex = 0;
		foreach(var gun in data.guns)
		{
			var cannon = Instantiate(canonImgPrefab) as Button;
			var img = cannon.GetComponent<Image>();
			cannon.transform.SetParent(cannonsHolder, false);
			
			Vector2 pos = cam.WorldToScreenPoint(spaceship.cacheTransform.position + (Vector3)gun.place.pos);
			img.rectTransform.anchoredPosition = pos - new Vector2(Screen.width, Screen.height)*0.5f;
			img.rectTransform.Rotate(new Vector3(0,0,1), Math2d.GetRotationDg(gun.place.dir));
			int cnnIndex = gunIndex;
			cannon.onClick.AddListener(() => cannonClicked(cnnIndex));
			cannons.Add(cannon);
			
			gunIndex++;
		}
	}

	public void Clear()
	{
		if (spaceship != null)
			Destroy (spaceship.gameObject);
		
		cannons.ForEach (c => Destroy (c.gameObject));
		cannons.Clear ();
	}
}
