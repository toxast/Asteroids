﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MParticleResources : ResourceSingleton<MParticleResources> {
	[SerializeField] public MParticleSystemsData iceParticles;
	[SerializeField] public MParticleSystemsData burnParticles;
	[SerializeField] public MParticleSystemsData powerUpDropParticles;
	[SerializeField] public MParticleSystemsData healOnceParticles;
	[SerializeField] public MParticleSystemsData healingParticles;
	[SerializeField] public MParticleSystemsData lightFreeParticles;
	[SerializeField] public MParticleSystemsData cometParticles;
	[SerializeField] public MParticleSystemsData cometDestroyParticles;
	[SerializeField] public MParticleSystemsData effectFinished;
}
