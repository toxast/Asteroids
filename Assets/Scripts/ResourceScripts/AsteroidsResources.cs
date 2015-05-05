using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AsteroidsResources : ResourceSingleton<AsteroidsResources> 
{
	[SerializeField] public List<AsteroidData> asteroidsData;
	[SerializeField] public List<AsteroidSetupData> asteroids;
	[SerializeField] public List<SpikeShooterInitData> spikyData;
	[SerializeField] public List<SawInitData> sawData;
}
