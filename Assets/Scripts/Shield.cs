using UnityEngine;
using System.Collections;

public class Shield 
{
	private ShieldData data;
	private float currentShields;
	private float time2startShieldRecharge;

	public Shield(ShieldData data)
	{
		this.data = data;
		currentShields = data.capacity;
		time2startShieldRecharge = 0;
	}

	public float Deflect(float dmg)
	{
		time2startShieldRecharge = data.rechargeDelay;
		if(currentShields > 0)
		{
			float deflected = 0;
			deflected = Mathf.Min(dmg, currentShields);
			currentShields -= deflected;
			if(currentShields <= 0)
			{
				currentShields = 0;
			}
			
			dmg -= deflected;
		}
		return dmg;
	}

	public void Tick(float delta)
	{
		if(time2startShieldRecharge > 0)
		{
			time2startShieldRecharge -= delta;
		}
		
		if(time2startShieldRecharge <= 0)
		{
			currentShields += delta * data.rechargeRate;
			if(currentShields >= data.capacity)
			{
				currentShields = data.capacity;
				time2startShieldRecharge = data.rechargeDelay;
			}
		}
	}
}
