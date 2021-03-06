﻿using UnityEngine;
using System.Collections;

public class oldGUI : MonoBehaviour {

	[SerializeField] Main main;
	string shipNum = string.Empty;
	string shipNum2 = string.Empty;
	public string userShipNum = "6";
	void OnGUI()
	{
		int starty = 20;
		int width = 120;
		int height = 20;
		int margine = 10;
		int y = starty;
		int x = 10;

#if !UNITY_STANDALONE
		height*=2;
		margine*=2;
#endif
		
		if(GUI.Button(new Rect(x, y, width+20, height), "asteroid"))
		{
		}
		y += height + margine;
		
		if(GUI.Button(new Rect(x, y, width, height), "saw"))
		{
		}
		y += height + margine;
		
		if(GUI.Button(new Rect(x, y, width, height), "spiky"))
		{
		}
		y += height + margine;
		
		if(GUI.Button(new Rect(x, y, width, height), "tank"))
		{
		}
		y += height + margine;
		
		if(GUI.Button(new Rect(x, y, width, height), "scout"))
		{
		}
		y += height + margine;
		
		if(GUI.Button(new Rect(x, y, width, height), "rogue"))
		{
		}
		y += height + margine;
		
		//second row 
		x += 2*width;
		y = starty;

		if(GUI.Button(new Rect(x, y, width, height), "simple tower"))
		{
		}
		y += height + margine;

		if(GUI.Button(new Rect(x, y, width, height), "block post"))
		{
			//main.CreateTower();
		}
		y += height + margine;


		shipNum = GUI.TextField (new Rect (x + 200, y, width - 20, height), shipNum);
		if(GUI.Button(new Rect(x, y, width+30, height), "enemy spaceship"))
		{
		}
		y += height + margine;

		shipNum2 = GUI.TextField (new Rect (x + 200, y, width - 20, height), shipNum2);
		if(GUI.Button(new Rect(x, y, width+30, height), "friend spaceship"))
		{
		}
	
		y += height + margine;
		userShipNum = GUI.TextField (new Rect (x + 200, y, width - 20, height), userShipNum);
		if(GUI.Button(new Rect(x, y, width, height), "respawn"))
		{
			StartCoroutine(main.Respawn());
		}
		y += height + margine;

//		if(GUI.Button(new Rect(x, y, width+30, height), "enemy spaceship 2"))
//		{
//			main.CreateEnemySpaceShipBoss();
//		}
//		y += height + margine;
		
		if(GUI.Button(new Rect(x, y, width+20, height), "gasteroid"))
		{
		}
		y += height + margine;

		if(GUI.Button(new Rect(x, y, width+20, height), "Fighters"))
		{
		}
		y += height + margine;
		

		y += height + margine;
		
		
		if(GUI.Button(new Rect(Screen.width-100, 10, width+20, height), "quit"))
		{
			Application.Quit();
		}
	}
}
