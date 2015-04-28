using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GUIHangar : MonoBehaviour 
{
	[SerializeField] Camera cam;
	[SerializeField] int ship = 1;
	[SerializeField] Transform cannonsHolder;
	[SerializeField] Button canonImgPrefab;
	int created = -1;

	[SerializeField] BuyShipElem buyShipElemPrefab;
	[SerializeField] RectTransform shipsHolder;

	[SerializeField] RectTransform rightScroll;

	PolygonGameObject spaceship;
	List<Button> cannons = new List<Button> ();

	void Start ()
	{
		List<int> indx = new List<int> ();

		for (int i = 0; i <  SpaceshipsResources.Instance.spaceships.Count; i++) {
			indx.Add(i);
		}

		for (int i = 0; i < indx.Count; i++) 
		{
			int shipIndex = indx[i];
			var sdata = SpaceshipsResources.Instance.spaceships [shipIndex];

			var shipElem = Instantiate(buyShipElemPrefab) as BuyShipElem;
			shipElem.transform.SetParent(shipsHolder, false);
			shipElem.Init(sdata.name, sdata.price); 
			shipElem.AddListener(() => Create(shipIndex));
		}
//		Create(ship);
//		created = ship;
	}
	
//	void Update () 
//	{
//		if(created > 0 && created != ship)
//		{
//			Create(ship);
//			created = ship;
//		}
//	}

	private void SelectCannon(int index)
	{
		Debug.LogWarning ("SelectCannon " + index);
	}

	private void Create(int shipIndx)
	{
		if (spaceship != null)
			Destroy (spaceship.gameObject);

		cannons.ForEach (c => Destroy (c.gameObject));
		cannons.Clear ();

		Debug.LogWarning ("create " + shipIndx);
		var sdata = SpaceshipsResources.Instance.spaceships [shipIndx];
		spaceship = PolygonCreator.CreatePolygonGOByMassCenter<PolygonGameObject> (sdata.verts, sdata.color);
		spaceship.cacheTransform.position = new Vector3(0, 0, 70);

		int gunIndex = 0;
		foreach(var gun in sdata.guns)
		{
			int cnnIndex = gunIndex;
			var cannon = Instantiate(canonImgPrefab) as Button;
			var img = cannon.GetComponent<Image>();
			cannon.transform.SetParent(cannonsHolder, false);

			Vector2 pos = cam.WorldToScreenPoint(spaceship.cacheTransform.position + (Vector3)gun.place.pos);
			img.rectTransform.anchoredPosition = pos - new Vector2(Screen.width, Screen.height)*0.5f;
			img.rectTransform.Rotate(new Vector3(0,0,1), Math2d.GetRotationDg(gun.place.dir));
			cannon.onClick.AddListener(() => SelectCannon(cnnIndex));
			cannons.Add(cannon);

			gunIndex++;
		}
	}
}
