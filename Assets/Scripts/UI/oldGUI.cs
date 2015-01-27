using UnityEngine;
using System.Collections;

public class oldGUI : MonoBehaviour {

	[SerializeField] Main main;
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
			main.CreateAsteroid();
		}
		y += height + margine;
		
		if(GUI.Button(new Rect(x, y, width, height), "saw"))
		{
			main.CreateSawEnemy();
		}
		y += height + margine;
		
		if(GUI.Button(new Rect(x, y, width, height), "spiky"))
		{
			main.CreateSpikyAsteroid();
		}
		y += height + margine;
		
		if(GUI.Button(new Rect(x, y, width, height), "tank"))
		{
			main.CreateTankEnemy();
		}
		y += height + margine;
		
		if(GUI.Button(new Rect(x, y, width, height), "scout"))
		{
			main.CreateEvadeEnemy();
		}
		y += height + margine;
		
		if(GUI.Button(new Rect(x, y, width, height), "rogue"))
		{
			main.CreateRogueEnemy();
		}
		y += height + margine;
		
		//second row 
		x += 2*width;
		y = starty;

		if(GUI.Button(new Rect(x, y, width, height), "simple tower"))
		{
			main.CreateSimpleTower();
		}
		y += height + margine;

		if(GUI.Button(new Rect(x, y, width, height), "block post"))
		{
			main.CreateTower();
		}
		y += height + margine;
		
		if(GUI.Button(new Rect(x, y, width+30, height), "enemy spaceship"))
		{
			main.CreateEnemySpaceShip();
		}
		y += height + margine;

		if(GUI.Button(new Rect(x, y, width+30, height), "enemy spaceship 2"))
		{
			main.CreateEnemySpaceShipBoss();
		}
		y += height + margine;
		
		if(GUI.Button(new Rect(x, y, width+20, height), "gasteroid"))
		{
			main.CreateGasteroid();
		}
		y += height + margine;
		
		if(GUI.Button(new Rect(x, y, width, height), "respawn"))
		{
			StartCoroutine(main.Respawn());
		}
		y += height + margine;
		
		
		if(GUI.Button(new Rect(Screen.width-100, 10, width+20, height), "quit"))
		{
			Application.Quit();
		}
	}
}
