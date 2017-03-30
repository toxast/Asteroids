using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace ui5
{
	[ExecuteInEditMode]
	[RequireComponent(typeof (Image))]
	public class ImageToggle5 : ToggleInteractable5
	{
		[SerializeField] Image image;

		[SerializeField] Sprite spriteOff;
		[SerializeField] Color off = Color.white;
		[SerializeField] Color offPassive = Color.white;

		[SerializeField] Sprite spriteOn;
		[SerializeField] Color on = Color.white;
		[SerializeField] Color onPassive = Color.white;

		protected override void OnValidate ()
		{
			if (image == null) 
			{
				image = GetComponent<Image> ();
				spriteOff = image.sprite;
                spriteOn = image.sprite;
				off = image.color;
				offPassive = image.color;
				on = image.color;
				onPassive = image.color;
            }
			base.OnValidate ();
		}

		protected override void Awake()
		{
            base.Awake();
			if (image == null) {
				image = GetComponent<Image> ();
				if (image != null) 
				{
					if(spriteOff == null)
						spriteOff = image.sprite;

					if(spriteOn == null)
						spriteOn = image.sprite;
				}
			}
		}

		protected override void RefreshView ()
		{
			image.sprite = _state ? spriteOn : spriteOff;
			if(_interactable)
				image.color = _state ? on : off;
			else
				image.color = _state ? onPassive : offPassive;
		}

	}
}
