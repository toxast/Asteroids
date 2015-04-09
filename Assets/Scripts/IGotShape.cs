using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface IGotShape
{
	Vector2[] iverts{ get; set;}
}

public interface IGotThrusters
{
	List<ThrusterSetupData> ithrusters {get; set;}
}

public interface IGotGuns
{
	List<GunSetupData> iguns {get; set;}
}

public interface IGotTurrets
{
	List<TurretReferenceData> iturrets {get; set;}
}

