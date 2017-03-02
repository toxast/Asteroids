using UnityEngine;
using System.Collections;

public class MGunBaseData : MonoBehaviour, IGun
{
	public string storeName;
	public int price;
	public GunSetupData.eGuns etype;

	public string iname{ get {return storeName;}}
	public int iprice{ get {return price;}}
	public virtual GunSetupData.eGuns itype{ get {return etype;}}

	public virtual Gun GetGun(Place setupData, PolygonGameObject t)
	{
		return null;
	}
}
