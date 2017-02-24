﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MSpawnSingle : MSpawnBase 
{
	[SerializeField] SpawnElem elem;

	public override float difficulty{ get{ return elem.difficulty; }}

	public override void Spawn(Vector2 pos, float lookAngle, Action<SpawnedObj> callback) {
		var main = Singleton<Main>.inst;
		main.StartCoroutine(SpawnRoutine (elem, pos, lookAngle, callback));
	}
}

