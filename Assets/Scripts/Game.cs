using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Game : MonoBehaviour 
{
	[SerializeField] Main main;
	[SerializeField] GUIHangar hangar;

	[SerializeField] List<GameObject> gameObjects;
	[SerializeField] List<GameObject> hangarObjects;

	void Awake()
	{
		hangarObjects.ForEach (h => h.SetActive (true));

		hangar.startTheGame += HandleStartTheGame;
	}

	void HandleStartTheGame (FullSpaceShipSetupData data)
	{
		hangarObjects.ForEach (h => h.SetActive (false));
		gameObjects.ForEach (h => h.SetActive (true));

		main.StartTheGame (data);
	}
}
