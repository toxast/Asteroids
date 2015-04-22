using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AsteroidsResources : ResourceSingleton<AsteroidsResources> 
{
	[SerializeField] public List<AsteroidData> asteroidsData;

	[SerializeField] public List<AsteroidResData> asteroidsResData;
	[SerializeField] public List<SpikyInitData> spikyData;
	[SerializeField] public List<SpikyInitData> sawData;

	[SerializeField] public List<RandomFloat> initSpeed;
	[SerializeField] public List<RandomFloat> initRotation;
	[SerializeField] public List<RandomFloat> initSize;
}
