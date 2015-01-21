using UnityEngine;
using System.Collections;

public interface InputController
{
	void Update (PolygonGameObject p);
	Vector2 TurnDirection ();
	bool IsShooting();
	bool IsAccelerating();
}
