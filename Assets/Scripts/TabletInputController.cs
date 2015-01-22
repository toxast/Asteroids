using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class TabletInputController : MonoBehaviour, InputController
{
	Rect flyZoneBounds;
	bool shooting = false;
	bool accelerating = false;
	Vector2 turnDirection;


	[SerializeField] Image joystick;
	[SerializeField] FireButton fireButton;
	[SerializeField] FireButton accelerateButton;
	[SerializeField] float controlRadius = 40f;
	float controlRadiusSqr;

	int fingerId = -1;
	//todo: getset;
	[System.NonSerialized] public Vector2 lastDisr = Vector2.zero;

	public void Init(Rect flyZoneBounds)
	{
		this.flyZoneBounds = flyZoneBounds;
		controlRadiusSqr = controlRadius*controlRadius;

		fireButton.gameObject.SetActive (true);
		accelerateButton.gameObject.SetActive (true);
		joystick.gameObject.SetActive (true);

		joystick.rectTransform.sizeDelta = new Vector2(2*controlRadius, 2*controlRadius);
	}
	
	public void Tick(PolygonGameObject p)
	{
		turnDirection = Vector2.zero;
		shooting = fireButton.pressed;
		accelerating = accelerateButton.pressed;
		
//		Vector2 moveTo = (Vector2)Camera.main.ScreenToWorldPoint (Input.mousePosition);
//		//moveTo = new Vector2 (Mathf.Clamp (moveTo.x, flyZoneBounds.xMin, flyZoneBounds.xMax
//		Vector2 curPos = p.cacheTransform.position;
//		//CheckIfShipOutOfBounds()
//		if(curPos.x < flyZoneBounds.xMin)
//		{
//			if(moveTo.x < flyZoneBounds.xMin)
//				moveTo.x = flyZoneBounds.xMin;
//			
//			accelerating = true;
//		}
//		else if(curPos.x > flyZoneBounds.xMax)
//		{
//			if(moveTo.x > flyZoneBounds.xMax)
//				moveTo.x = flyZoneBounds.xMax;
//			
//			accelerating = true;
//		}
//		
//		if(curPos.y < flyZoneBounds.yMin)
//		{
//			if(moveTo.y < flyZoneBounds.yMin)
//				moveTo.y = flyZoneBounds.yMin;
//			
//			accelerating = true;
//		}
//		else if(curPos.y > flyZoneBounds.yMax)
//		{
//			if(moveTo.y > flyZoneBounds.yMax)
//				moveTo.y = flyZoneBounds.yMax;
//			
//			accelerating = true;
//		}
		
		var touches = new List<Touch>(Input.touches);
		foreach (var tch in touches) 
		{
			Vector2 dir = tch.position - (Vector2)joystick.rectTransform.position;
			if(dir.sqrMagnitude < controlRadiusSqr)
			{
				turnDirection = dir;
				break;
			}
		}

//		if(Input.GetMouseButton(0))
//		{
//			Vector2 moveTo = Input.mousePosition;
//			Vector2 dir = moveTo - (Vector2)joystick.rectTransform.position;
//			if(dir.sqrMagnitude < controlRadiusSqr)
//			{
//				turnDirection = dir;
//			}
//			else
//			{
//				turnDirection = Vector2.zero;
//			}
//		}
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
