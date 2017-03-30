using UnityEngine;
using System.Collections;

namespace ui5
{
	public class ToggleInteractable5 : Toggle5
	{
		[SerializeField] protected bool _interactable = true;
		
		public override bool IsInteractable
		{
			get {return _interactable;}
		}
		
		public override void SetInteractable(bool interactable)
		{
			if (_interactable != interactable) 
			{
				_interactable = interactable;

				foreach (var item in linked) {
					item.SetInteractable(_interactable);
				}

				RefreshView();
			}
		}

		public bool Passive
		{
			get{return !_interactable;}
			set{SetInteractable(!value);}
		}
	}
}
