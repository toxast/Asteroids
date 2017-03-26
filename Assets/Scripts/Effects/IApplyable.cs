using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IApplyable {
	IHasProgress Apply (PolygonGameObject picker);
}
