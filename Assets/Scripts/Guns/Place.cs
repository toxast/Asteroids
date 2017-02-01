﻿using UnityEngine;
using System.Collections;

[System.Serializable]
public class Place: IClonable<Place>
{
	public Vector2 pos = Vector2.zero;
	public Vector2 dir = new Vector2(1,0);
	public Place(){}
	public Place(Vector2 pos, Vector2 dir)
	{
		this.pos = pos;
		this.dir = dir;
	}

	public Place Clone()
	{
		return new Place (pos, dir);
	}

}
