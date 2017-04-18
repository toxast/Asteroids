using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UserComboPowerup : IApplyable, IHasDuration
{
	public Color color = Color.white;
	public float overrideEffectsDuration = -1;
	public List<MEffectData> effects;
	public float iduration {
		get {
			var ed = effects.Find( e => (e is IHasDuration));
			if (ed != null) {
				return (ed as IHasDuration).iduration;
			} else {
				return 0;
			}
		} 
		set {
			foreach (var item in effects) {
				if (item is IHasDuration) {
					(item as IHasDuration).iduration = value;
				}
			}
		}
	}

	public IHasProgress Apply (PolygonGameObject picker) {
		if (overrideEffectsDuration >= 0) {
			iduration = overrideEffectsDuration;
		}
		IHasProgress progressEffect = null;
		foreach (var item in effects) {
			var effect = item.Apply (picker);
			if (effect != null && progressEffect == null) {
				progressEffect = effect;
			}
		}

		ReplaceDropCollectorEffect.Data collector = new ReplaceDropCollectorEffect.Data {
			duration = 4f,
			force = 0.5f,
			range = 80f
		};
		collector.Apply (picker);


		if (progressEffect != null) {
			var desParticles = MParticleResources.Instance.effectFinished.data.Clone();
			desParticles.startColor = color;
			EffectDeathAnimation.Data onEffectFinish = new EffectDeathAnimation.Data {
				effect = progressEffect,
				particles = new List<ParticleSystemsData> {desParticles}
			};
			onEffectFinish.Apply (picker);
				
			ProgressColorWrapper wrapper = new ProgressColorWrapper{ progressObj = progressEffect, color = color };
			Singleton<Main>.inst.DisplayPowerup (wrapper);
		}

		return progressEffect;
	}

	public class ProgressColorWrapper: IHasProgress
	{
		public IHasProgress progressObj;
		public Color color;

		public float iprogress{ get { return progressObj.iprogress; } }
	
	}
}

