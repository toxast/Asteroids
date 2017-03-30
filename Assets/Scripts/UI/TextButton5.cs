using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace ui5
{
	[System.Serializable]
	public class TextButton5 : MonoBehaviour 
	{
		public Button button;
		public Text label;

		public Button.ButtonClickedEvent onClick
		{
			get{return button.onClick;}
			set{button.onClick = value;}
		}

		public string text
		{
			get{return label.text;}
			set{label.text = value;}
		}
	}
}
