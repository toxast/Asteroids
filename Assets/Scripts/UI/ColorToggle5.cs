using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace ui5
{
	[ExecuteInEditMode]
	public class ColorToggle5 : Toggle5 
	{
		[SerializeField] Graphic elem;
		[SerializeField] Color off;
		[SerializeField] Color on;

		protected override void Awake()
		{
            base.Awake();
			if (elem == null) 
			{
				elem = GetComponent<Graphic> ();
				off = elem.color;
				on = elem.color;
			}
		}

		protected override void RefreshView ()
		{
            if (elem == null)
				elem = GetComponent<Graphic> ();

            if ( elem == null )
                return;

			elem.color = _state ? on : off;
		}
	}
}
