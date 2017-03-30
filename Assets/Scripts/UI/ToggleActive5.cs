using UnityEngine;
using System.Collections;

namespace ui5
{
	public class ToggleActive5 : Toggle5
	{
		[SerializeField] bool reverseLogic = false;

		protected override void RefreshView ()
		{
			if(!reverseLogic)
				gameObject.SetActive (_state);
			else
				gameObject.SetActive (!_state);
		}
	}
}
