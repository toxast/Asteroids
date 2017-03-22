using UnityEngine;
using System.Collections;

public class MGunBaseData : MonoBehaviour
{
	[SerializeField] protected float dps;

	protected virtual void OnValidate(){
		dps = CalculateDps();
	}

	protected virtual float CalculateDps() {return 0;}

	public virtual Gun GetGun(Place setupData, PolygonGameObject t)
	{
		return null;
	}
}
