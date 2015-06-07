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
	[SerializeField] InputField levelInput;
	[SerializeField] InputField waveInput;
	[SerializeField] List<GameObject> gameObjects;
	[SerializeField] List<GameObject> hangarObjects;

	void Awake()
	{
		hangarObjects.ForEach (h => h.SetActive (true));

		hangar.startTheGame += HandleStartTheGame;

		main.gameOver += HandleGameOver;
		main.levelCleared += HandlelevelCleared;
	}

	void HandlelevelCleared ()
	{
		Debug.LogWarning ("levelCleared");
		StartCoroutine(FinishGameIn (10f));
	}

	void HandleGameOver ()
	{
		Debug.LogWarning ("GameOver");
		StartCoroutine (FinishGameIn (3f));
	}

	IEnumerator FinishGameIn(float seconds)
	{
		yield return new WaitForSeconds (seconds);

		hangarObjects.ForEach (h => h.SetActive (true));
		gameObjects.ForEach (h => h.SetActive (false));

		yield return new WaitForEndOfFrame ();
		yield return new WaitForEndOfFrame ();

		main.Clear ();
	}

	void HandleStartTheGame (FullSpaceShipSetupData data)
	{
		hangarObjects.ForEach (h => h.SetActive (false));
		gameObjects.ForEach (h => h.SetActive (true));

		main.StartTheGame (data, int.Parse(levelInput.text), int.Parse(waveInput.text));
	}
}

/*
 * анимация появления врагов
 * дроп со всех
 * апгрейд кораблей
 * болле детальные параметры для всех врагов 
 * радиус появления врагов
 * появления в главном меню после сметри
 * сохранение игры
 */