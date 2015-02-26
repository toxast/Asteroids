using UnityEngine;
using System.Collections;

[System.Serializable]
public class GunSetupData : IClonable<GunSetupData>
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

	public GunSetupData Clone()
	{
		GunSetupData r = new GunSetupData ();
		r.place = place.Clone ();
		r.type = type;
		r.index = index; 
		return r;
	}
}
