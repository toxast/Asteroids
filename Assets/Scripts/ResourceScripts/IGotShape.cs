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
	List<MGunSetupData> iguns {get; set;}
}

public interface IGotTurrets
{
	List<MTurretReferenceData> iturrets {get; set;}
}

