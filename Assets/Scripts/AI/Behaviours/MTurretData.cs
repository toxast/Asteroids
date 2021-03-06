﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class MTurretData : MonoBehaviour , IGotShape, IGotGuns
{
	public Color color = Color.white;
	public float rotationSpeed = 50;
	public float restrictionAngle = 360;
	public float repeatTargetCheck = 1f;
	public AccuracyData accuracy;
	public List<MGunSetupData> guns;
	public List<List<int>> linkedGuns;
	public Vector2[] verts;

    //interfaces
    public Vector2[] iverts {get {return verts;} set{verts = value;}}
	public List<MGunSetupData> iguns {get {return guns;} set{guns = value;}}
}
