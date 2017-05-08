using UnityEngine;
using System.Collections;

public class MGunBaseData : MonoBehaviour
{
	[SerializeField] protected float dps;
	[SerializeField] protected float collisionDmg;
	[SerializeField] protected float estimateRange;

	protected virtual void OnValidate(){
		dps = CalculateDps();
		estimateRange = CalculateRange ();
	}

	protected virtual float CalculateDps() {return 0;}
	protected virtual float CalculateRange() {return 0;}

	public virtual Gun GetGun(Place setupData, PolygonGameObject t)
	{
		return null;
	}
}
