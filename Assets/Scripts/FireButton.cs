using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FireButton : Button {

	public bool pressed = false;

	public override void OnPointerDown (UnityEngine.EventSystems.PointerEventData eventData)
	{
		base.OnPointerDown (eventData);
		pressed = true;
	}

	public override void OnPointerUp (UnityEngine.EventSystems.PointerEventData eventData)
	{
		base.OnPointerUp (eventData);
		pressed = false;
	}
}
