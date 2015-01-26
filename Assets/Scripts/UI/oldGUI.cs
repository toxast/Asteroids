using UnityEngine;
using System.Collections;

public class oldGUI : MonoBehaviour {

	[SerializeField] Main main;
	void OnGUI()
	{
		int starty = 20;
		int width = 100;
		int hieight = 40;
		int margine = 20;
		int y = starty;
		int x = 10;
		
		if(GUI.Button(new Rect(x, y, width+20, hieight), "asteroid"))
		{
			main.CreateAsteroid();
		}
		y += hieight + margine;
		
		if(GUI.Button(new Rect(x, y, width, hieight), "saw"))
		{
			main.CreateSawEnemy();
		}
		y += hieight + margine;
		
		if(GUI.Button(new Rect(x, y, width, hieight), "spiky"))
		{
			main.CreateSpikyAsteroid();
		}
		y += hieight + margine;
		
		if(GUI.Button(new Rect(x, y, width, hieight), "tank"))
		{
			main.CreateTankEnemy();
		}
		y += hieight + margine;
		
		if(GUI.Button(new Rect(x, y, width, hieight), "scout"))
		{
			main.CreateEvadeEnemy();
		}
		y += hieight + margine;
		
		if(GUI.Button(new Rect(x, y, width, hieight), "rogue"))
		{
			main.CreateRogueEnemy();
		}
		y += hieight + margine;
		
		//second row 
		x += 2*width;
		y = starty;
		
		if(GUI.Button(new Rect(x, y, width, hieight), "tower"))
		{
			main.CreateTower();
		}
		y += hieight + margine;
		
		if(GUI.Button(new Rect(x, y, width, hieight), "enemy spaceship"))
		{
			main.CreateEnemySpaceShip();
		}
		y += hieight + margine;
		
		if(GUI.Button(new Rect(x, y, width+20, hieight), "gasteroid"))
		{
			main.CreateGasteroid();
		}
		y += hieight + margine;
		
		if(GUI.Button(new Rect(x, y, width, hieight), "respawn"))
		{
			StartCoroutine(main.Respawn());
		}
		y += hieight + margine;
		
		
		if(GUI.Button(new Rect(Screen.width-100, 10, width+20, hieight), "quit"))
		{
			Application.Quit();
		}
	}
}
