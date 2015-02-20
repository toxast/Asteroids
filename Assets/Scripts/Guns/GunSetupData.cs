using UnityEngine;
using System.Collections;

[System.Serializable]
public class GunSetupData
{
	public enum eGuns
	{
		BULLET,
		ROCKET,
		SPAWNER,
	}

	public Place place;
	public eGuns type;
	public int index;
}
