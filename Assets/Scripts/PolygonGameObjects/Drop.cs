﻿using UnityEngine;
using System.Collections;

namespace polygonGO
{
	public class Drop : PolygonGameObject 
	{
		public float lifetime;

		public override void Tick (float delta)
		{
			base.Tick (delta);
			lifetime -= delta;
			if(lifetime < 0)
			{
				currentHealth = 0;
			}
		}
	}
}
