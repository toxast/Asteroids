using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MEffectData : MonoBehaviour, IApplyable{
	public abstract IHasProgress Apply (PolygonGameObject picker);
}

