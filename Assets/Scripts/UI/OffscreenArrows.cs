using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffscreenArrows : MonoBehaviour {
	[SerializeField] RectTransform arrowPrefab;
	[SerializeField] Transform container;

	float arrowHalfSize = 10f;
	List<RectTransform> arrows = new List<RectTransform> ();

	void Awake(){
		arrowPrefab.CreatePool (5);
	}

	public void Display(List<Main.OffScreenPosition> positions) {
		int diff = positions.Count - arrows.Count;
		if (diff > 0) {
			for (int i = 0; i < diff; i++) {
				arrows.Add(arrowPrefab.Spawn (container));//Instantiate (arrowPrefab, container));
			}
		} else if (diff < 0) {
			int toRemove = -diff;
			for (int i = 0; i < toRemove; i++) {
				arrows [arrows.Count - 1 - i].gameObject.Recycle ();
				//Destroy (arrows [arrows.Count - 1 - i].gameObject);
			}
			arrows.RemoveRange (arrows.Count - toRemove, toRemove);
		}

		for (int i = 0; i < positions.Count; i++) {
			var pos = positions [i];
			var arrow = arrows [i];
			switch (pos.side) {
			case Main.OffScreenPosition.eSide.DOWN:
				arrow.anchoredPosition = new Vector2 (Screen.width * pos.pos01, -Screen.height*0.5f + arrowHalfSize);
				arrow.localRotation = Quaternion.Euler(new Vector3(0, 0, 180));
				break;
			case Main.OffScreenPosition.eSide.TOP:
				arrow.anchoredPosition = new Vector2 (Screen.width * pos.pos01, Screen.height*0.5f  - arrowHalfSize);
				arrow.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
				break;
			case Main.OffScreenPosition.eSide.RIGHT:
				arrow.anchoredPosition = new Vector2 (Screen.width*0.5f - arrowHalfSize, Screen.height * pos.pos01);
				arrow.localRotation = Quaternion.Euler(new Vector3(0, 0, -90));
				break;
			case Main.OffScreenPosition.eSide.LEFT:
				arrow.anchoredPosition = new Vector2 (-Screen.width*0.5f + arrowHalfSize , Screen.height * pos.pos01);
				arrow.localRotation = Quaternion.Euler(new Vector3(0, 0, 90));
				break;
			}	
		}
	}
}
