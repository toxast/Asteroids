﻿using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Game : MonoBehaviour 
{
	[SerializeField] Main main;
	[SerializeField] GUIHangar hangar;
	[SerializeField] InputField waveInput;
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

		main.StartTheGame (data, int.Parse(waveInput.text));
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