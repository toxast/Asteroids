using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class TabletInputController : MonoBehaviour, InputController
{
	public bool shooting{ get; private set; }
	public bool accelerating{ get; private set; }
	public bool braking{ get; private set; }
	public Vector2 turnDirection{ get; private set; }
	public float accelerateValue01{ get{ return 1f;}} 

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

	public void Freeze(float m){ }

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

    public void SetSpawnParent(PolygonGameObject prnt) { }
}
