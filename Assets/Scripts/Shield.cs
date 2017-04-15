using UnityEngine;
using System.Collections;

[System.Serializable]
public class Shield 
{
	private ShieldData data;
	public float capacity{get{return data.capacity;}}
	public float currentShields{ get; private set; }
	private float time2startShieldRecharge;

	private PolygonGameObject shieldGo;


	private Coroutine shieldAnimationCoroutine = null;
	private const float FADE_FURATION = 0.3f;
	private const float NO_SHIELD_ALPHA = 0f;
	private const float SHIELD_ALPHA = 0.3f;
	private const float SHIELD_HIT_ALPHA = 0.8f;//0.6f;

	public Shield(ShieldData data, PolygonGameObject shieldGo)
	{
		this.data = data;
		this.shieldGo = shieldGo;
		currentShields = capacity;
		time2startShieldRecharge = 0;
	}

	//returns not deflected dmg
	public float Deflect(float dmg)
	{
		if (currentShields > 0) {
			float deflected = 0;
			deflected = Mathf.Min (dmg, currentShields);
			currentShields -= deflected;
			if (currentShields <= 0) {
				currentShields = 0;
			}
			dmg -= deflected;
			if (shieldGo != null) {
				if (currentShields <= 0) {
					SetAlpha (NO_SHIELD_ALPHA);
				} else if (deflected > 0) {
					shieldAnimationCoroutine = shieldGo.StartCoroutine (AnimateHit ());
				}
			}

			if (currentShields <= 0) {
				time2startShieldRecharge = data.rechargeDelayAfterDestory;
			}
		}

		time2startShieldRecharge = Mathf.Max (data.hitRechargeDelay, time2startShieldRecharge);

		return dmg;
	}

	private void SetAlpha(float a)
	{
		if (shieldAnimationCoroutine != null) {
			shieldGo.StopCoroutine (shieldAnimationCoroutine);
		}
		shieldGo.SetAlpha(a);
	}

	public void Tick(float delta)
	{
		if (time2startShieldRecharge > 0) {
			time2startShieldRecharge -= delta;
			if (shieldGo != null && time2startShieldRecharge <= 0) {
				shieldAnimationCoroutine = shieldGo.StartCoroutine (FadeTo (shieldGo, SHIELD_ALPHA, FADE_FURATION));
			}
		}
		
		if (time2startShieldRecharge <= 0 && currentShields < capacity) {
			currentShields += delta * data.rechargeRate;
			if (currentShields >= capacity) {
				currentShields = capacity;
			}
		}
	}

	IEnumerator AnimateHit()
	{
		shieldGo.SetAlpha(SHIELD_HIT_ALPHA);
		shieldAnimationCoroutine = shieldGo.StartCoroutine(FadeTo (shieldGo, SHIELD_ALPHA, FADE_FURATION));
		yield return shieldAnimationCoroutine;
	}

	IEnumerator FadeTo(PolygonGameObject p, float alpha, float fadeTime)
	{
		float currentAlpha = p.mesh.colors [0].a;
		bool greater = alpha > currentAlpha;
		float dAlpha = (alpha - currentAlpha);
		while (true) {
			currentAlpha = p.mesh.colors [0].a;
			float newAlpha = Mathf.Clamp (currentAlpha + (Time.deltaTime / fadeTime) * dAlpha, 0f, 1f);
			p.SetAlpha (newAlpha);
			if (greater) {
				if (newAlpha >= alpha)
					break;
			} else {
				if (newAlpha <= alpha)
					break;
			}
			yield return new WaitForSeconds (0f); 
		}
	}
}
