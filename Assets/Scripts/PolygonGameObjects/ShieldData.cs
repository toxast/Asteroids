using UnityEngine;
using System.Collections;

[System.Serializable]
public class ShieldData: IClonable<ShieldData>
{
	public float capacity;
	public float rechargeRate;
	public float hitRechargeDelay;
	public float rechargeDelayAfterDestory = 1; 

	public ShieldData()
	{
	}

	public ShieldData(float capacity, float rechargeRate, float rechargeDelay, float rechargeDelayAfterDestory)
	{
		this.capacity = capacity;
		this.rechargeRate = rechargeRate;
		this.hitRechargeDelay = rechargeDelay;
		this.rechargeDelayAfterDestory = rechargeDelayAfterDestory;
	}

	public ShieldData Clone()
	{
		return new ShieldData (capacity, rechargeRate, hitRechargeDelay, rechargeDelayAfterDestory);
	}
}
