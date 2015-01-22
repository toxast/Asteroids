using UnityEngine;
using System.Collections;

public class StandaloneInputController : InputController
{
	Rect flyZoneBounds;
	bool shooting = false;
	bool accelerating = false;
	Vector2 turnDirection;

	public StandaloneInputController(Rect flyZoneBounds)
	{
		this.flyZoneBounds = flyZoneBounds;
	}

	public void Tick(PolygonGameObject p)
	{
		shooting = Input.GetMouseButton (0);
		accelerating = Input.GetKey (KeyCode.W);

		Vector2 moveTo = (Vector2)Camera.main.ScreenToWorldPoint (Input.mousePosition);
		//moveTo = new Vector2 (Mathf.Clamp (moveTo.x, flyZoneBounds.xMin, flyZoneBounds.xMax
		Vector2 curPos = p.cacheTransform.position;
		//CheckIfShipOutOfBounds()
		if(curPos.x < flyZoneBounds.xMin)
		{
			if(moveTo.x < flyZoneBounds.xMin)
				moveTo.x = flyZoneBounds.xMin;

			accelerating = true;
		}
		else if(curPos.x > flyZoneBounds.xMax)
		{
			if(moveTo.x > flyZoneBounds.xMax)
				moveTo.x = flyZoneBounds.xMax;

			accelerating = true;
		}

		if(curPos.y < flyZoneBounds.yMin)
		{
			if(moveTo.y < flyZoneBounds.yMin)
				moveTo.y = flyZoneBounds.yMin;

			accelerating = true;
		}
		else if(curPos.y > flyZoneBounds.yMax)
		{
			if(moveTo.y > flyZoneBounds.yMax)
				moveTo.y = flyZoneBounds.yMax;

			accelerating = true;
		}

		turnDirection = moveTo - curPos;
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
