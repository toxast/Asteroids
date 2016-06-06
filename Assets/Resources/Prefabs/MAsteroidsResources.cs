using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MAsteroidsResources : ResourceSingleton<MAsteroidsResources> 
{
	[SerializeField] public List<MAsteroidData> asteroidsData;
	[SerializeField] public List<MSpikyData> spikyData;
	[SerializeField] public List<MSawData> sawData;
}
