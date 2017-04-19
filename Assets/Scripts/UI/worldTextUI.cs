using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class worldTextUI : MonoBehaviour {
	[SerializeField] Canvas canvas;
	[SerializeField] Text floatingTextPrefab;
	[SerializeField] RectTransform container;
	[SerializeField] public bool showDmgInEditor = true;
	void Awake() {
		floatingTextPrefab.CreatePool (10);
	}

	bool enabledScript = false;
	void OnEnable(){
		enabledScript = true;
		floatingTextPrefab.RecycleAll ();
	}

	void OnDisable(){
		enabledScript = false;
	}

	public void ShowText(Vector3 pos, Color color, string text, int size = 15) {
		if (!enabledScript) {
			return;
		}
		var textElem = floatingTextPrefab.Spawn (container);
		textElem.color = color;
		textElem.text = text;
		textElem.fontSize = size;
		textElem.transform.position = pos;
		textElem.StartCoroutine (TextAnimation (textElem));
		textElem.StartCoroutine (TextRizeAnimation (textElem));
	}

	IEnumerator TextRizeAnimation(Text textElem) {
		while (true) {
			textElem.transform.position += 2f * Time.deltaTime * Vector3.up;
			yield return null;
		}
	}

	IEnumerator TextAnimation(Text textElem) {
		yield return new WaitForSeconds (1.5f);
		float duration = 1.5f;
		float left = duration;
		while (left > 0) {
			left -= Time.deltaTime;
			var color = textElem.color;
			color.a = Mathf.Clamp01 (left / duration);
			textElem.color = color;
			yield return null;
		}
		textElem.color = new Color(0,0,0,0);
		yield return null;
		textElem.Recycle ();
	}

}
