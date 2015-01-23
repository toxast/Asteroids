using UnityEngine;
using System.Collections;
using UnityEngine.UI;

//[ExecuteInEditMode]
public class HealthBar : MonoBehaviour {

	[SerializeField] Image back;
	[SerializeField] Image bar;

	private float width;

	void Awake()
	{
		width = back.rectTransform.sizeDelta.x;
		GameResources.healthChanged += DisplayHealth;
	}

	public void DisplayHealth(float persent)
	{
		bar.rectTransform.anchoredPosition = new Vector3 ((persent-1f) * width, 0);
	}

//	[SerializeField] float p;
//	void Update()
//	{
//		SetHealth (p);
//	}
}
