using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class TabletInputController : MonoBehaviour, InputController
{
	bool shooting = false;
	bool accelerating = false;
	bool braking = false;
	Vector2 turnDirection;


	[SerializeField] Image joystick;
	[SerializeField] ToggleButton fireButton;
	[SerializeField] FireButton accelerateButton;
	[SerializeField] float controlRadius = 40f;
	float controlRadiusSqr;

	//int fingerId = -1;
	//todo: getset;
	[System.NonSerialized] public Vector2 lastDisr = Vector2.zero;


	public void Init()
	{
		controlRadiusSqr = controlRadius*controlRadius;

		fireButton.gameObject.SetActive (true);
		accelerateButton.gameObject.SetActive (true);
		joystick.gameObject.SetActive (true);

		joystick.rectTransform.sizeDelta = new Vector2(2*controlRadius, 2*controlRadius);
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
	
	public bool IsBraking()
	{
		return braking;
	}

	public void Tick(PolygonGameObject p)
	{
		turnDirection = Vector2.zero;
		shooting = fireButton.pressed;
		accelerating = accelerateButton.pressed;
		braking = true;

		var touches = new List<Touch>(Input.touches);
		foreach (var tch in touches) 
		{
			Vector2 dir = tch.position - (Vector2)joystick.rectTransform.position;
			if(dir.sqrMagnitude < 8*controlRadiusSqr)
			{
				turnDirection = dir;
				braking = false;
				break;
			}
		}
	}
	

}
