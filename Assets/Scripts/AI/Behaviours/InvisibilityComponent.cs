using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class InvisibilityComponent: ITickable {
	[Serializable]
	public class Data {
		public float fadeOutDuration = 1f; 
		public float fadeInDuration = 0.6f; 
		public float slowerFadeOnHit = 5f;
	}

	PolygonGameObject parent;
	bool shouldBeInvisible = false;
	float fadeInSpeedPerSecond;

	float fadeOutSpeedPerSecond;
	float fadeOutAfterHitSpeedPerSecond;
	float currentfadeOutSpeed;

	public InvisibilityComponent(PolygonGameObject parent, Data invisData){
		this.parent = parent;

		fadeOutSpeedPerSecond = 1f / invisData.fadeOutDuration;
		fadeInSpeedPerSecond = 1f /invisData.fadeInDuration;
		fadeOutAfterHitSpeedPerSecond = fadeOutSpeedPerSecond / invisData.slowerFadeOnHit;
		currentfadeOutSpeed = fadeOutSpeedPerSecond;
	}

	public void SetState(bool invisible){
		shouldBeInvisible = invisible;
		currentfadeOutSpeed = fadeOutSpeedPerSecond;
	}

	public void Tick (float delta) 	{
		var currentAlpha = parent.GetAlpha();
		if (shouldBeInvisible) {
			if (currentAlpha > 0) {
				float newAlpha = Mathf.Clamp(currentAlpha - currentfadeOutSpeed * delta, 0f, 1f);
				parent.SetAlphaAndInvisibility(newAlpha);
				if (newAlpha == 0) {
					currentfadeOutSpeed = fadeOutAfterHitSpeedPerSecond;
				}
			}
		} else {
			if (currentAlpha < 1) {
				float newAlpha = Mathf.Clamp(currentAlpha + fadeInSpeedPerSecond * delta, 0f, 1f);
				parent.SetAlphaAndInvisibility(newAlpha);
			}
		}
	}
}
