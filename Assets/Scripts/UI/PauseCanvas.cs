using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseCanvas : MonoBehaviour {
	[SerializeField] Canvas canvas;
	[SerializeField] Button resumeBtn;
	[SerializeField] Button menuBtn;

	public event Action OnResume;
	public event Action OnMenu;

	public void Show() {
		Visible = true;
	}

	bool Visible{
		get{return canvas.enabled;}
		set{
			canvas.enabled = value;
			Time.timeScale = value ? 0 : 1;
		}
	}

	void Awake(){
		resumeBtn.onClick.AddListener (HandleResume);
		menuBtn.onClick.AddListener (HandleMenu);
	}

	void HandleResume() {
		Visible = false;
		if (OnResume != null) {
			OnResume ();
		}
	}

	void HandleMenu() {
		Visible = false;
		if (OnMenu != null) {
			OnMenu ();
		}
	}
}
