using UnityEngine;
using System.Collections;

[System.Serializable]
public class TurretReferenceData : IClonable<TurretReferenceData>, IGotPlace
{
	public Place place;
	public int index;
	
	public Place pos {get {return place;} set{place = value;}}
	
	public TurretReferenceData Clone()
	{
		TurretReferenceData r = new TurretReferenceData ();
		r.place = place.Clone ();
		r.index = index; 
		return r;
	}
}

[System.Serializable]
public class MTurretReferenceData : IGotPlace
{
	public Place place;
	public MTurretData turret;

	public Place pos {get {return place;} set{place = value;}}
}