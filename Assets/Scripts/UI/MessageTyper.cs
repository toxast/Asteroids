using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageTyper : MonoBehaviour {
	[SerializeField] Text msgField;
	[SerializeField] ScrollRect scroll;
	[SerializeField] float interval = 0.2f;
	[SerializeField] float intervalNextSentence = 0.6f;

	Queue<string> queue = new Queue<string> ();

	Coroutine routine;
	public void Show(string msg){
		msg =  msg.Replace ("\\n", "\n");
		msg =  msg.Replace ("\n ", "\n");
		if (routine != null) {
			queue.Enqueue (msg);
		} else {
			Clear ();
			routine = StartCoroutine (Typer (msg));
		}
	}

	public void Clear(){
		if (routine != null) {
			StopCoroutine (routine);
			routine = null;
		}
		msgField.text = "";
		scroll.SetTopOffset (scroll.transform as RectTransform, 0);
	}

	private void OnTyperFinished() {
		routine = null;
		if (queue.Count > 0) {
			string next = queue.Dequeue ();
			if (msgField.text != string.Empty) {
				next = "\n\n" + next;
			}
			routine = StartCoroutine (Typer (next));
		}
	}

	IEnumerator Typer(string msg) {
		string startText = msgField.text;
		int currIndex = 0;
		float lineHeight = 24;
		float lastDelta = msgField.rectTransform.sizeDelta.y;
		while (currIndex < msg.Length) {
			if (lastDelta != msgField.rectTransform.sizeDelta.y) {
				lastDelta = msgField.rectTransform.sizeDelta.y;
				if (scroll.BottomOffset () > 0) { 
					float duration = 0.33f;
					float speed = lineHeight / duration;
					while (duration > 0) {
						var offset = Mathf.Max (0, scroll.BottomOffset () - Time.deltaTime * speed);
						scroll.SetBottomOffset (scroll.transform as RectTransform, offset);
						duration -= Time.deltaTime;
						yield return null;
					}
				}
			}

			if (currIndex > 0 && msg [currIndex-1] == ' ' && msg [currIndex] == ' ') {
				msg = msg.Remove (currIndex, 1);
				yield return new WaitForSeconds (interval);
			} else {
				float curInterval = interval;
				if (currIndex > 0 && msg [currIndex-1] == '.' && (msg [currIndex] == ' ' || msg [currIndex] == '\n')) {
					curInterval = intervalNextSentence;
				} 
				msgField.text = startText + msg.Substring (0, currIndex + 1);
				currIndex++;
				yield return new WaitForSeconds (curInterval);
			}
		}
		OnTyperFinished ();
	}

}
