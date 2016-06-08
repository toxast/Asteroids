using UnityEngine;
using System.Collections;

[System.Serializable]
public class MGunSetupData : IGotPlace
{
	public Place place;
	public MGunBaseData gun;
	public Place pos {get {return place;} set{place = value;}}

	public Gun GetGun(PolygonGameObject t)
	{
		return gun.GetGun(place, t);
	}

}
