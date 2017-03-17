using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
	[SerializeField] Image back;
	[SerializeField] Image bar;
	
	private float width;
	
	protected virtual void Awake() {
		width = back.rectTransform.sizeDelta.x;
	}
	
	protected void Display(float persent) {
        persent = Mathf.Clamp01(persent);
		bar.rectTransform.anchoredPosition = new Vector3 ((persent - 1f) * width, 0);
	}
}
