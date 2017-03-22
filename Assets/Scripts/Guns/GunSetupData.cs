using UnityEngine;
using System.Collections;

[System.Serializable]
public class GunSetupData : IClonable<GunSetupData>, IGotPlace
{
	public enum eGuns
	{
		None = 0,
		BULLET = 1,
		ROCKET = 2,
		SPAWNER = 3,
		LAZER = 4,
		TURRET = 5,
        FLAME = 6,
	}

	public Place place;
	public eGuns type;
	public int index;

	public Place pos {get {return place;} set{place = value;}}

	public GunSetupData Clone()
	{
		GunSetupData r = new GunSetupData ();
		r.place = place.Clone ();
		r.type = type;
		r.index = index; 
		return r;
	}
}