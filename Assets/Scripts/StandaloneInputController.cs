using UnityEngine;
using System.Collections;

public class StandaloneInputController : InputController
{
	Rect flyZoneBounds;
	bool shooting = false;
	bool accelerating = false;
	Vector2 turnDirection;

	private float pushingForce = 4f;

	public StandaloneInputController(Rect flyZoneBounds)
	{
		this.flyZoneBounds = flyZoneBounds;
	}

	private void ApplyForce(PolygonGameObject p, Vector2 dir)
	{
		Vector3 f = dir.normalized * pushingForce * dir.sqrMagnitude;
		p.velocity += (Time.deltaTime * f) / p.mass ;
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
			//TODO: delta
			float dir = (flyZoneBounds.xMin - curPos.x);
			ApplyForce(p, new Vector2(dir,0));
		}
		else if(curPos.x > flyZoneBounds.xMax)
		{
			float dir = (flyZoneBounds.xMax - curPos.x);
			ApplyForce(p, new Vector2(dir,0));
		}

		if(curPos.y < flyZoneBounds.yMin)
		{
			float dir = (flyZoneBounds.yMin - curPos.y);
			ApplyForce(p, new Vector2(0,dir));
		}
		else if(curPos.y > flyZoneBounds.yMax)
		{
			float dir = (flyZoneBounds.yMax - curPos.y);
			ApplyForce(p, new Vector2(0,dir));
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
