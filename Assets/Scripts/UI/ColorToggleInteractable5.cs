using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace ui5
{
	[ExecuteInEditMode]
	[RequireComponent(typeof (Graphics))]
	public class ColorToggleInteractable5 : ToggleInteractable5 
	{
		[SerializeField] Graphic elem;

		[SerializeField] Color off = Color.white;
		[SerializeField] Color offPassive = Color.white;
		[SerializeField] Color on = Color.white;
		[SerializeField] Color onPassive = Color.white;


		protected override void OnValidate ()
		{
			if (elem == null) 
			{
				elem = GetComponent<Image> ();
				off = elem.color;
				offPassive = elem.color;
				on = elem.color;
				onPassive = elem.color;
			}
			base.OnValidate ();
		}


        protected override void Awake()
		{
            base.Awake();
			if (elem == null) 
			{
				elem = GetComponent<Graphic> ();
			}
		}

		protected override void RefreshView ()
		{
			if(_interactable)
				elem.color = _state ? on : off;
			else
				elem.color = _state ? onPassive : offPassive;
		}
	}
}
