using UnityEngine;
using System.Collections;

namespace polygonGO
{
	public class Drop : PolygonGameObject 
	{
		public float lifetime;
		public MAsteroidCommonData data;
		bool moneyAdded = false;

		public override void Tick (float delta)
		{
			base.Tick (delta);
			lifetime -= delta;
			if(lifetime < 0)
			{
				if(!moneyAdded)
				{
					GameResources.AddMoney(data.value);
					moneyAdded = true;
				}
				currentHealth = 0;
			}
		}

		public void Collect()
		{
			if(!moneyAdded)
			{
				GameResources.AddMoney(data.value);
				moneyAdded = true;
			}
		}
	}
}
