﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MGunsShowElement : MonoBehaviour, IGotGuns{
	public List<MGunSetupData> guns;
	public List<List<int>> linkedGuns;
	public float rotation = 100f;

	//interfaces
	public List<MGunSetupData> iguns { get { return guns; } set { guns = value; } }
}