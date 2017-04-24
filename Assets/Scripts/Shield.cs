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
					SetAlpha (NO_SHIELD_ALPHA, true);
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

	float currentMultiplyAlpha = 1;
	float currentShieldAlpha = 0;

	//used for invisible 
	public void MultiplyAlpha(float a) {
		currentMultiplyAlpha = a;
		UpdateAlpha ();
	}

	void SetAlpha(float a, bool stopCoroutine){
		currentShieldAlpha = a;
		if (stopCoroutine && shieldAnimationCoroutine != null) {
			shieldGo.StopCoroutine (shieldAnimationCoroutine);
		}
		UpdateAlpha ();
	}

	void UpdateAlpha(){
		shieldGo.SetAlpha(currentMultiplyAlpha * currentShieldAlpha);
	}

	public void Tick(float delta)
	{
		if (time2startShieldRecharge > 0) {
			time2startShieldRecharge -= delta;
			if (shieldGo != null && time2startShieldRecharge <= 0) {
				shieldAnimationCoroutine = shieldGo.StartCoroutine (FadeTo (SHIELD_ALPHA, FADE_FURATION));
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
		SetAlpha(SHIELD_HIT_ALPHA, false);
		shieldAnimationCoroutine = shieldGo.StartCoroutine(FadeTo (SHIELD_ALPHA, FADE_FURATION));
		yield return shieldAnimationCoroutine;
	}

	IEnumerator FadeTo(float alpha, float fadeTime)
	{
		float currentAlpha = currentShieldAlpha;
		bool greater = alpha > currentAlpha;
		float dAlpha = (alpha - currentAlpha);
		while (true) {
			currentAlpha = currentShieldAlpha;
			float newAlpha = Mathf.Clamp (currentAlpha + (Time.deltaTime / fadeTime) * dAlpha, 0f, 1f);
			SetAlpha (newAlpha, false);
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
