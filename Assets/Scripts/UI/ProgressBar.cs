using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
	[SerializeField] Image back;
	[SerializeField] Image bar;
	
	private float width;
	private float lastPersent = 0;

	protected virtual void Start() {
		RefreshWidth ();
	}

	void OnEnable() {
		RefreshWidth ();
	}

	void RefreshWidth(){
		width = back.rectTransform.rect.width;
		Display (lastPersent);
	}

	public void SetBarColor(Color col){
		bar.color = col;
	}
	
	public void Display(float persent) {
		lastPersent = persent;
        persent = Mathf.Clamp01(persent);
		bar.rectTransform.anchoredPosition = new Vector3 ((persent - 1f) * width, 0);
	}
}
