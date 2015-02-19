using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ToggleButton : Button {
	
	public bool pressed = false;
	
	public override void OnPointerDown (UnityEngine.EventSystems.PointerEventData eventData)
	{
		base.OnPointerDown (eventData);
		pressed = !pressed;
	}
	

}