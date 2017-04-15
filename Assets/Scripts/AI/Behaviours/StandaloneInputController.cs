﻿using UnityEngine;
using System.Collections;
using System.Linq;

public class StandaloneInputController : InputController
{
	public bool shooting{ get; private set; }
	public bool accelerating{ get; private set; }
	public bool braking{ get; private set; }
	public Vector2 turnDirection{ get; private set; }
	public float accelerateValue01{ get; private set;} 
	public void Freeze(float m){ }
	bool autofireIsOn = true;

	public StandaloneInputController()
	{
	}

	public void Tick(PolygonGameObject p)
	{
		if (hasXboxConnected) {
			if(Input.GetButton ("Xbox_RB")){
				autofireIsOn = !autofireIsOn;
			}	
			shooting = autofireIsOn ^ Input.GetButton ("Xbox_X");

			float min = 0.14f;
			float fullAt = 0.7f;
			var trigger = Input.GetAxis ("Xbox_RT");
			accelerating = trigger > min;

			float minAccelerate = 0.3f;
			if (accelerating) {
				accelerateValue01 = minAccelerate + (1f - minAccelerate) * ((trigger - min) / (fullAt - min));
				accelerateValue01 = Mathf.Clamp01 (accelerateValue01);
			} else {
				accelerateValue01 = 0;
			}

			braking = Input.GetButton ("Xbox_Y");
			float x = Input.GetAxis ("HorizontalGamepad");
			float y = Input.GetAxis ("VerticalGamepad");
			turnDirection = new Vector2 (x, y).normalized;
		} else {
			accelerateValue01 = 1;
//			if(Input.GetButtonDown(KeyCode.R)){
//				autofireIsOn = !autofireIsOn;
//			}
			shooting = autofireIsOn ^ Input.GetMouseButton (0);
			accelerating = Input.GetKey (KeyCode.W);
			braking = Input.GetKey (KeyCode.S);


			Vector2 moveTo = (Vector2)Camera.main.ScreenToWorldPoint (Input.mousePosition);
			//moveTo = new Vector2 (Mathf.Clamp (moveTo.x, flyZoneBounds.xMin, flyZoneBounds.xMax

			turnDirection = moveTo - p.position;
		}
	}

	bool hasXboxConnected{
		get{ 
			return Input.GetJoystickNames ().ToList ().Exists (j => j.ToLower ().Contains ("xbox 360"));
		}
	}

    public void SetSpawnParent(PolygonGameObject prnt) { }

}
