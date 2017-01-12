using UnityEngine;
using System.Collections;

public interface InputController
{
	void Tick (PolygonGameObject p);
	Vector2 turnDirection{ get;}
	bool shooting{get;}
	bool accelerating{ get;}
	bool braking{get;}
    void SetSpawnParent(PolygonGameObject prnt);
}
