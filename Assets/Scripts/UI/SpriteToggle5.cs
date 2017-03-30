using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SpriteToggle5 : ui5.Toggle5
{
	[SerializeField] Image image;
	[SerializeField] Sprite spriteOff;
	[SerializeField] Sprite spriteOn;

	protected override void OnValidate ()
	{
		if (image == null) 
		{
			image = GetComponent<Image> ();
			spriteOff = image.sprite;
			spriteOn = image.sprite;
		}
		base.OnValidate ();
	}

	protected override void Awake()
	{
        base.Awake();
		if (image == null) {
			image = GetComponent<Image> ();
		}
	}

	protected override void RefreshView ()
	{
		image.sprite = _state ? spriteOn : spriteOff;
	}


}
