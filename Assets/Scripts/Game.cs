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

		main.OnGameOver += HandleGameOver;
		main.OnLevelCleared += HandlelevelCleared;
	}

	void HandlelevelCleared ()
	{
		Debug.LogWarning ("levelCleared");
		FinishGameIn (1f);
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

	void HandleStartTheGame (MSpaceshipData data)
	{
		hangarObjects.ForEach (h => h.SetActive (false));
		gameObjects.ForEach (h => h.SetActive (true));

		main.StartTheGame (data, MPowerUpResources.Instance.powerups[0].comets, int.Parse(levelInput.text), int.Parse(waveInput.text));
	}
}

/*
 * сохранение игры
 */