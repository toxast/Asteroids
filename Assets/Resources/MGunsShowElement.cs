using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MGunsShowElement : MonoBehaviour, IGotGuns{
	public List<MGunSetupData> guns;
	public List<int> linkedGuns;
	public float rotation = 100f;
	public Place offset;
	//interfaces
	public List<MGunSetupData> iguns { get { return guns; } set { guns = value; } }
}