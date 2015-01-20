using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Joystick : MonoBehaviour {

	Image joystick;
	float maxOffset; //TODO: sets
	Vector2 joystickPos = Vector2.zero;
	int fingerId = -1;

	//todo: getset;
	[System.NonSerialized] public Vector2 lastDisr = Vector2.zero;

	public bool IsPressing 
	{
		get{
			return fingerId >= 0;
		}
	}

	public void Set(Image joystick, float maxOffset)
	{
		this.joystick = joystick;
		this.maxOffset = maxOffset;
	}

	void Update()
	{
		var touches = new List<Touch>(Input.touches);
		if(fingerId < 0 && Input.touchCount > 0)
		{
			touches.Sort( (t1, t2) => t1.position.x.CompareTo(t2.position.x) );
			var mostLeftTouch = touches[0];
			if(mostLeftTouch.position.x < Screen.width/2f)
			{
				fingerId = touches[0].fingerId;
			}
			else
			{
				fingerId = -1;
			}
		}

		if(fingerId >= 0)
		{
			Touch touch = new Touch();
			int indx = touches.FindIndex (t => t.fingerId == fingerId);
			if(indx < 0)
			{
				//joystick disappears
				lastDisr = Vector2.zero;
				fingerId = -1;
				joystick.enabled = false;
			}
			else
			{
				touch = touches[indx];
				if(touch.phase == TouchPhase.Began)
				{
					//position joystic
					joystickPos = touch.position;
					if(joystick != null)
					{
						joystick.enabled = true;
						joystick.rectTransform.position = joystickPos;
						joystick.rectTransform.sizeDelta = new Vector2(2*maxOffset, 2*maxOffset);
					}
				}
				
				//calculate move
				lastDisr = (Vector2)touch.position - joystickPos;
			}
		}
		
	}
}
