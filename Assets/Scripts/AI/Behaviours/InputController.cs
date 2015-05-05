using UnityEngine;
using System.Collections;

public interface InputController
{
	void Tick (PolygonGameObject p);
	Vector2 TurnDirection ();
	bool IsShooting();
	bool IsAccelerating();
	bool IsBraking();
}
