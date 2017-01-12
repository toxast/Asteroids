using UnityEngine;
using System.Collections;

public class StandaloneInputController : InputController
{
	public bool shooting{ get; private set; }
	public bool accelerating{ get; private set; }
	public bool braking{ get; private set; }
	public Vector2 turnDirection{ get; private set; }


	public StandaloneInputController()
	{
	}

	public void Tick(PolygonGameObject p)
	{
		shooting = Input.GetMouseButton (0);
		accelerating = Input.GetKey (KeyCode.W);
		braking = Input.GetKey (KeyCode.S);
		Vector2 moveTo = (Vector2)Camera.main.ScreenToWorldPoint (Input.mousePosition);
		//moveTo = new Vector2 (Mathf.Clamp (moveTo.x, flyZoneBounds.xMin, flyZoneBounds.xMax

		turnDirection = moveTo - p.position;
	}

    public void SetSpawnParent(PolygonGameObject prnt) { }

}
