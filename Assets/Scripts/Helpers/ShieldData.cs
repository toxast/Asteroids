using UnityEngine;
using System.Collections;

[System.Serializable]
public class ShieldData: IClonable<ShieldData>
{
	public float capacity;
	public float rechargeRate;
	public float rechargeDelay;

	public ShieldData()
	{
	}

	public ShieldData(float capacity, float rechargeRate, float rechargeDelay)
	{
		this.capacity = capacity;
		this.rechargeRate = rechargeRate;
		this.rechargeDelay = rechargeDelay;
	}

	public ShieldData Clone()
	{
		return new ShieldData (capacity, rechargeRate, rechargeDelay);
	}
}
