using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectUI : MonoBehaviour {
	[SerializeField] ui5.TextButton5 LevelButton;
	[SerializeField] Button previous;
	[SerializeField] Button next;

	public event Action<int> OnStart;
	int avaliableLevelIndex = 0;
	int currentIndex = 0;

	void Awake(){
		previous.onClick.AddListener (HandlePrevious);
		next.onClick.AddListener (HandleNext);
		LevelButton.onClick.AddListener (Go);
	}

	public void Refresh(int avaliableLevelIndex, int totalLevelsCount) {
		this.avaliableLevelIndex = Mathf.Min(avaliableLevelIndex, totalLevelsCount - 1);
		SetOnIndx (avaliableLevelIndex);
	}

	void SetOnIndx(int indx) {
		currentIndex = Mathf.Clamp (indx, 0, avaliableLevelIndex);
		LevelButton.text = "Level " + (currentIndex + 1);
		previous.interactable = currentIndex > 0;
		next.interactable = currentIndex < avaliableLevelIndex;
	}

	void HandlePrevious() {
		SetOnIndx (currentIndex - 1);
	}

	void HandleNext() {
		SetOnIndx (currentIndex + 1);
	}

	void Go() {
		if (OnStart != null) {
			OnStart (currentIndex);
		}
	}
}
