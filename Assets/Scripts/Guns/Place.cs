using UnityEngine;
using System.Collections;

[System.Serializable]
public class Place
{
	public Vector2 pos;
	public Vector2 dir;
	public Place(){}
	public Place(Vector2 pos, Vector2 dir)
	{
		this.pos = pos;
		this.dir = dir;
	}

}
