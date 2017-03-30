using UnityEngine;
using System.Collections;
using System;

namespace ui5
{
	public interface IToggle5 
	{
        void On();
        void Off();
		bool IsOn{ get;}
		bool IsInteractable{get;}
		void SetInteractable(bool interactable);
		void SetState(bool on);
		void Toggle();
	}
}
