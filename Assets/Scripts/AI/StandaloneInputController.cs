using UnityEngine;
using System.Collections;

public class StandaloneInputController : InputController
{
	bool shooting = false;
	bool accelerating = false;
	Vector2 turnDirection;



	public StandaloneInputController()
	{
	}

	public void Tick(PolygonGameObject p)
	{
		shooting = Input.GetMouseButton (0);
		accelerating = Input.GetKey (KeyCode.W);

		Vector2 moveTo = (Vector2)Camera.main.ScreenToWorldPoint (Input.mousePosition);
		//moveTo = new Vector2 (Mathf.Clamp (moveTo.x, flyZoneBounds.xMin, flyZoneBounds.xMax

		turnDirection = moveTo - (Vector2)p.cacheTransform.position;
	}

	public Vector2 TurnDirection ()
	{
		return turnDirection;
	}

	public bool IsShooting()
	{
		return shooting;
	}

	public bool IsAccelerating()
	{
		return accelerating;
	}
}
