using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthBoss : PolygonGameObject 
{
	MEarthBossData data;
	AdvancedTurnComponent turnComponent;
	SpawnedArmData leftArm;
	SpawnedArmData rightArm;
	PolygonGameObject helmet;

	public void Init(MEarthBossData data){
		this.data = data;
		turnComponent = new AdvancedTurnComponent (this, data.rotationSpeed);
		Debug.LogError ("TODO: show health");
		Debug.LogError ("TODO: dmg distribution");
		Debug.LogError ("TODO: user attacks from back ? rotation attcks or !back invurnabitity! - helm parts, head slightly different color");
		Debug.LogError ("TODO: effects");
		Debug.LogError ("TODO: second minigun attack");
		Debug.LogError ("TODO: minigun attack has followers rockets or something");
	}

	List<Gun> guns1;
	List<ParticleSystem> minigunEffects;
	bool showRightArmParticles = true;

	bool overrideTurnDir;
	Vector2 turnDir = new Vector2(1,0);
	bool isAttacking = false;

	bool decreaseVelocity = true;

	public override void BeforeAddToMainList () {
		base.BeforeAddToMainList ();

		helmet = data.helmetPrefab.Create ();
		Math2d.PositionOnParentAbs (helmet.cacheTransform, data.helmetPlace, cacheTransform, true, -0.5f);
		Singleton<Main>.inst.Add2Objects (helmet);
		this.AddObjectAsFollower (helmet);

		leftArm = CreateArm (data.leftArm, false);
		rightArm = CreateArm (data.rightArm, true);

		guns1 = new List<Gun> ();
		var gun = data.gun.GetGun (leftArm.hand.obj);
		guns1.Add (gun);
		leftArm.hand.obj.AddExtraGuns (guns1);

		var minigunHand = leftArm.hand.obj;
		minigunEffects = minigunHand.AddParticles (data.minigunHandAttackEffect);
		minigunEffects.ForEach (p => p.Stop ());

		StartCoroutine (IncreaseRotationAtStart ());
		StartCoroutine (AttacksRoutine ());
	}

	SpawnedArmData CreateArm(MEarthBossData.ArmData armData, bool right){
		var shoulder = CreatePart (armData.shoulder, this);
		var joint = CreatePart (armData.joint, shoulder.obj);
		var hand = CreatePart (armData.hand, joint.obj);
		{
			var anim = data.effectArm2Head;
			var place = anim.effect.place;
			if (right) {
				place.pos.y = -place.pos.y;
			}
			var effectInst = shoulder.obj.AddParticles (new List<ParticleSystemsData>{ anim.effect })[0];
			StartCoroutine (DirectEffect (effectInst, this.transform));
		}
		{
			var effectInst = joint.obj.AddParticles (new List<ParticleSystemsData>{ data.jointEffect })[0];
			StartCoroutine (DirectEffect (effectInst, shoulder.obj.transform));
		}
		{
			var effectInst = hand.obj.AddParticles (new List<ParticleSystemsData>{ data.handEffect })[0];
			if (right) {
				StartCoroutine (DirectEffect (effectInst, joint.obj.transform, ShowRightArmParticles));
			} else {
				StartCoroutine (DirectEffect (effectInst, joint.obj.transform));
			}
		}

		return new SpawnedArmData{ shoulder = shoulder, joint = joint, hand = hand, state = MEarthBossData.ArmState.NORMAL, right = right };
	}

	SpawnedJointData CreatePart(MEarthBossData.JointData jointData, PolygonGameObject relativeTo){
		var obj = jointData.spawn.Create ();
		this.AddObjectAsFollower (obj);
		obj.position = relativeTo.position;
		var angleEff = obj.AddEffect(new KeepOrientationEffect(jointData.orientationData, relativeTo));
		var posEff = obj.AddEffect(new KeepPositionEffect(jointData.positionData, relativeTo));
		Singleton<Main>.inst.Add2Objects (obj);
		return new SpawnedJointData{ obj = obj, angleEff = angleEff, posEff = posEff };
	}


	protected override void ApplyRotation (float dtime) {
		if (overrideTurnDir) {
			turnComponent.TurnByDirection (turnDir, dtime);
		} else if (TargetNotNull) {
			turnComponent.TurnByDirection (target.position - this.position, dtime);
		} else {
			turnComponent.TurnByDirection (cacheTransform.right, dtime);
		}
	}

	private void HealToFullHealth(PolygonGameObject pg){
		if(!Mathf.Approximately(pg.fullHealth, pg.CurrentHealth())) {
			pg.Heal (pg.fullHealth - pg.CurrentHealth());
		}
	}

	float lastDelta = 0.03f;
	public override void Tick (float delta)	{
		base.Tick (delta);
		lastDelta = delta;

		HealToFullHealth (leftArm.hand.obj);
		HealToFullHealth (leftArm.joint.obj);
		HealToFullHealth (leftArm.shoulder.obj);
		HealToFullHealth (rightArm.hand.obj);
		HealToFullHealth (rightArm.joint.obj);
		HealToFullHealth (rightArm.shoulder.obj);
		HealToFullHealth (helmet);

		var leftArmState = data.overrideArmsState && Application.isEditor ? data.leftArmState : leftArm.state;
		var rightArmState = data.overrideArmsState && Application.isEditor ? data.rightArmState : rightArm.state;

		ControlJointAngle (leftArm.shoulder, leftArmState, -data.armNormalContractAngle, -data.armMaxContractAngle, delta);
		ControlJointAngle (leftArm.joint, leftArmState, -data.jointNormalContractAngle, -data.jointMaxContractAngle, delta);
		ControlJointAngle (rightArm.shoulder, rightArmState, data.armNormalContractAngle, data.armMaxContractAngle, delta);
		ControlJointAngle (rightArm.joint, rightArmState, data.jointNormalContractAngle, data.jointMaxContractAngle, delta);

		if (data.chargeHitAttack) {
			data.chargeHitAttack = false;
			StartCoroutine (ChargeHitAttack ());
		}

		if (data.throwArmAttack) {
			data.throwArmAttack = false;
			StartCoroutine (FireArmAttack ());
		}

		if (data.shootAttck) {
			data.shootAttck = false;
			StartCoroutine (ShootAttack ());
		}

		if (data.shootAttack2) {
			data.shootAttack2 = false;
			StartCoroutine (EnumShootAttack2 ());
		}

		if (!Main.IsNull (helmet)) {
			helmet.velocity = this.velocity;
			Math2d.PositionOnParentAbs (helmet.cacheTransform, data.helmetPlace, cacheTransform, true, -0.5f);
		}

		if(decreaseVelocity) {
			Brake (delta, 7);
		}
	}

	void SetAttack(bool attack){
		//Debug.LogError ("attack " + attack);
		isAttacking = attack;
	}

	int attack = UnityEngine.Random.Range (0, 3);
	IEnumerator AttacksRoutine(){
		yield return new WaitForSeconds (4f);
		while (true) {
			attack += UnityEngine.Random.Range (1, 2);
			if (attack > 3) {
				attack -= 4;
			}
			if (attack == 0) {
				StartCoroutine (ChargeHitAttack ());
			} else if (attack == 1) {
				StartCoroutine (FireArmAttack ());
			} else if (attack == 2) {
				StartCoroutine (ShootAttack ());
			} else if (attack == 3) {
				StartCoroutine (EnumShootAttack2 ());
			} 
			yield return null;
			while (isAttacking) {
				yield return null;
			}
			leftArm.state = MEarthBossData.ArmState.NORMAL;
			rightArm.state = MEarthBossData.ArmState.NORMAL;
			yield return new WaitForSeconds(1f);
		}
	}

	IEnumerator IncreaseRotationAtStart(){
		float mul = 3f;
		MultiplyBossMovements (mul);

		float duration = 3f;
		while (duration > 0) {
			duration -= lastDelta;
			yield return null;
		}

		mul = 1f / mul;
		MultiplyBossMovements (mul);
	}

	void MultiplyBossMovements(float mul){
		leftArm.shoulder.MultiplyEffects(mul);
		leftArm.joint.MultiplyEffects(mul);
		leftArm.hand.MultiplyEffects(mul);
		rightArm.shoulder.MultiplyEffects(mul);
		rightArm.joint.MultiplyEffects(mul);
		rightArm.hand.MultiplyEffects(mul);
	}

	bool ShowRightArmParticles(){
		return showRightArmParticles;
	}

	IEnumerator DirectEffect(ParticleSystem ps, Transform towardsObject, System.Func<bool> visible = null){
		Transform effectTr = ps.transform;
		while (true) {
			if (effectTr == null || towardsObject == null) {
				break;				
			}
			if (visible != null) {
				bool enable = visible ();
				if (ps.gameObject.activeSelf && !enable) {
					ps.gameObject.SetActive(false);
				} else if(!ps.gameObject.activeSelf && enable){
					ps.gameObject.SetActive(true);
					ps.Play ();
				}
			}
			effectTr.forward = towardsObject.position - effectTr.position;
			yield return true;
		}
	} 

	IEnumerator ChargeHitAttack(){
		SetAttack(true);
		decreaseVelocity = false;
		float fasterRotationkff = data.rotationMultiplier;
		turnComponent.MultiplyOriginalTurnSpeed (fasterRotationkff);
		MultiplyBossMovements (fasterRotationkff);


		float duration = 3f;
		leftArm.state = MEarthBossData.ArmState.CONTRACT;
		rightArm.state = MEarthBossData.ArmState.EXTEND;
		overrideTurnDir = true;
		while (duration >= 0 && TargetNotNull) {
			turnDir = Math2d.MakeRight(target.position - this.position);
			duration -= lastDelta;
			yield return true;
		}

		if(TargetNotNull) {
			this.velocity = (target.position - this.position).normalized * 17f;
			leftArm.state = MEarthBossData.ArmState.NORMAL;
			duration = 6f;
			while (duration >= 0 && TargetNotNull) {
				float angle = 90f;
				turnDir = Math2d.RotateVertexDeg(cacheTransform.right, angle);
				duration -= lastDelta;
				yield return true;
			}
		}

		float rollback = 1f / fasterRotationkff;
		turnComponent.MultiplyOriginalTurnSpeed (rollback);
		MultiplyBossMovements (rollback);


		overrideTurnDir = false;
		SetAttack(false);
		decreaseVelocity = true;
	}

	IEnumerator FireArmAttack(){
		SetAttack(true);
		overrideTurnDir = true;
		float duration = 5f; //5
		float minAimDuration = 0.5f; //0.5
		float vel = 35;
		float velSqr = vel * vel;

		leftArm.state = MEarthBossData.ArmState.CONTRACT;
		while (duration > 0 && TargetNotNull) {
			float angle = 30f;
			turnDir = Math2d.RotateVertexDeg(target.position - position, angle);
			rightArm.state = GetArmAim(target.position, target.velocity, rightArm, vel, 0.7f);
			if (duration > minAimDuration && rightArm.state == MEarthBossData.ArmState.HOLD) {
				duration = Mathf.Min (minAimDuration, duration);
				showRightArmParticles = false;
			}
			duration -= lastDelta;
			yield return true;
		}
		showRightArmParticles = false;

		var hammer = rightArm.hand.obj;
		rightArm.hand.ToggleEffects(false);
		RemoveFollower (rightArm.hand.obj);
		duration = 4f;
		float continueAccelerateDuration = 0.5f;

		while (duration > 0) {
			hammer.Accelerate (lastDelta, 50, 1, vel, velSqr, hammer.cacheTransform.right);
			duration -= lastDelta;
			if (duration > continueAccelerateDuration && (TargetIsNull || Vector2.Dot (target.position - hammer.position, hammer.cacheTransform.right) < 0)) {
				duration = Mathf.Min(continueAccelerateDuration, duration);
			}
			yield return true;
		}

		vel = 25;
		velSqr = vel * vel;
		if(TargetNotNull) {
			duration = 3.5f;
			float sign = Vector2.Dot((target.position - hammer.position), hammer.cacheTransform.right) > 0 ? 1 : -1;
			var rotationData = new KeepRotationEffect.Data{acceleration = 100f, targetRotation = 300 * sign};
			var rotationEffect = new KeepRotationEffect (rotationData);
			hammer.AddEffect (rotationEffect);
			while (duration > 0 && TargetNotNull) {
				hammer.Accelerate (lastDelta, 20f, 0.5f, vel, velSqr, (target.position - hammer.position).normalized);
				duration -= lastDelta;
				yield return true;
			}

			rotationData.targetRotation = 0;
			duration = 1.5f;
			while (duration > 0) {
				duration -= lastDelta;
				yield return true;
			}

			duration = 5f;
			while (duration > 0 && (hammer.position - rightArm.joint.obj.position).magnitude > 20f) {
				hammer.Accelerate (lastDelta, 20f, 1f, vel, velSqr, (rightArm.joint.obj.position - hammer.position).normalized);
				duration -= lastDelta;
				yield return true;
			}
			rotationEffect.forceFinish = true;
		}
		AddObjectAsFollower (rightArm.hand.obj);
		rightArm.hand.ToggleEffects(true);
		overrideTurnDir = false;
		showRightArmParticles = true;
		SetAttack(false);
	}

	IEnumerator ShootAttack(){
		SetAttack(true);
		overrideTurnDir = true;
		float attkDuration = 13; 
		float delay = 2f;
		var minigunHand = leftArm.hand.obj;
		rightArm.state = MEarthBossData.ArmState.NORMAL;
		float angleTurn = UnityEngine.Random.Range(-30f, 30f);
		Vector3 pos = Vector3.zero;
		minigunEffects.ForEach (p => p.Play ());
		while (attkDuration > 0 && TargetNotNull) {
			turnDir = Math2d.RotateVertexDeg(target.position - position, angleTurn);
			leftArm.state = GetArmAim (target.position, target.velocity, leftArm, guns1[0].BulletSpeedForAim, 1f);
			if (delay <= 0) {
				foreach (var item in guns1) {
					item.Tick (lastDelta);
					bool anyShoot = false;
					if (item.ReadyToShoot ()) {
						anyShoot = true;
						item.ShootIfReady ();
					}
					if (anyShoot) {
						minigunHand.velocity -= (Vector2)minigunHand.cacheTransform.right * 20f;
					}
				}
			}
			if (delay > 0) {
				//todo: animation
				delay -= lastDelta;
			} else {
				attkDuration -= lastDelta;
			}
			yield return true;
		}
		leftArm.state = MEarthBossData.ArmState.NORMAL;
		overrideTurnDir = false;
		minigunEffects.ForEach (p => p.Stop ());
		SetAttack(false);
	}

	int attackRoutinesSpawned = 0;
	IEnumerator EnumShootAttack2(){
		attackRoutinesSpawned++;
		SetAttack(true);
		float hitTime;
		overrideTurnDir = false;
		float duration = data.shoot2duration; 
		float delay = 2f;
		float untilShoot = 0;
		var minigunHand = leftArm.hand.obj;
		rightArm.state = MEarthBossData.ArmState.NORMAL;
		Vector3 pos = Vector3.zero;
		minigunEffects.ForEach (p => p.Play ());

		while (duration > 0 && TargetNotNull) {
			leftArm.state = GetArmAim (target.position, target.velocity, leftArm, data.shoot2aimVelocity, 1f, out hitTime);
			if (delay <= 0) {
				untilShoot -= lastDelta;
				if (untilShoot <= 0 && leftArm.state == MEarthBossData.ArmState.HOLD) {
					untilShoot = data.shoot2delay;
					List<PolygonGameObject> objs = new List<PolygonGameObject> ();
					for (int i = 0; i < 3; i++) {
						var obj = data.shoot2Spawn.Create ();
						Math2d.PositionOnParent (obj.cacheTransform, data.shoot2spawnPlace, minigunHand.cacheTransform, false, 0.1f); 
						obj.destroyOnBoundsTeleport = true;
						objs.Add (obj);
						Singleton<Main>.inst.Add2Objects (obj);
					}
					objs [0].velocity = minigunHand.cacheTransform.right * data.shoot2aimVelocity * 0.8f;
					objs [1].velocity = Math2d.RotateVertexDeg(minigunHand.cacheTransform.right, data.shoot2SpreadDeg) * data.shoot2aimVelocity;
					objs [2].velocity = Math2d.RotateVertexDeg(minigunHand.cacheTransform.right, -data.shoot2SpreadDeg) * data.shoot2aimVelocity;
					attackRoutinesSpawned++;
					StartCoroutine(ControlShootAttack2Objects(objs));
				}
			}
			if (delay > 0) {
				delay -= lastDelta;
			} else {
				duration -= lastDelta;
			}
			yield return true;
		}
		leftArm.state = MEarthBossData.ArmState.NORMAL;
		minigunEffects.ForEach (p => p.Stop ());
		attackRoutinesSpawned--;
		if(attackRoutinesSpawned == 0){
			SetAttack(false);
		}
	}

	IEnumerator ControlShootAttack2Objects(List<PolygonGameObject> objs){
		float duration = data.shoot2forceDuration;
		Vector2 dir = objs [0].velocity.normalized;
		Vector2 dirRight = Math2d.MakeRight (dir);
		Vector2 dirLeft = -dirRight;
		List<Vector2> dirs = new List<Vector2>{ dir, dirRight, dirLeft };
		while (duration > 0) {
			for (int i = 1; i < objs.Count; i++) {
				var obj = objs [i];
				if (!Main.IsNull (obj)) {
					obj.Accelerate (lastDelta, data.shoot2accelerateForce, data.shoot2SaccStabl, data.shoot2maxVel, data.shoot2maxVel * data.shoot2maxVel, dirs [i]);
				}
			}
			duration -= lastDelta;
			yield return true;
		}
		attackRoutinesSpawned--;
		if(attackRoutinesSpawned == 0){
			SetAttack(false);
		}
	}


	MEarthBossData.ArmState GetArmAim(Vector2 tpos, Vector2 speed, SpawnedArmData arm, float bulletsSpeed = 0, float accuracy = 1){
		float hitTime;
		return GetArmAim (tpos, speed, arm, bulletsSpeed, accuracy, out hitTime);
	}

	MEarthBossData.ArmState GetArmAim(Vector2 tpos, Vector2 speed, SpawnedArmData arm, float bulletsSpeed, float accuracy, out float hitTime){
		hitTime = -1;
		Vector2 armDir = arm.hand.obj.cacheTransform.right; // - arm.joint.obj.position).normalized;
		Vector2 targetDir =   tpos - arm.hand.obj.position;
		if (bulletsSpeed != 0) {
			AimSystem aim = new AimSystem (tpos, speed * accuracy, arm.hand.obj.position, bulletsSpeed);
			if (aim.canShoot) {
				targetDir = aim.directionDist;
				hitTime = aim.time;
			}
		}
		
		float deltaAngle = Math2d.DegBetweenNormUnsigned (armDir, targetDir.normalized);
		if (deltaAngle > data.shootAngle) {
			var sign = Mathf.Sign (Math2d.Cross (ref armDir, ref targetDir));
			if (!arm.right) {
				sign = -sign;
			}
			if (sign < 0) {
				return MEarthBossData.ArmState.EXTEND;
			} else {
				return MEarthBossData.ArmState.CONTRACT;
			}
		} else {
			return MEarthBossData.ArmState.HOLD;
		}
	}

	void ControlJointAngle(SpawnedJointData joint, MEarthBossData.ArmState state, float normalAngle, float maxAngle, float delta) {
		var angleEff = joint.angleEff;
		float desiredOffsetAngle = angleEff.extraOffsetAngle;
		float deltaAngle = delta * angleEff.rotaitingSpeed;
		switch (state) {
		case MEarthBossData.ArmState.HOLD:
			break;
		case MEarthBossData.ArmState.CONTRACT:
			desiredOffsetAngle = maxAngle;
			break;
		case MEarthBossData.ArmState.EXTEND:
			desiredOffsetAngle = 0;
			break;
		case MEarthBossData.ArmState.NORMAL:
			desiredOffsetAngle = normalAngle;
			break;
		default:
			break;
		}
		if (angleEff.extraOffsetAngle != desiredOffsetAngle) {
			angleEff.extraOffsetAngle = MoveTo (angleEff.extraOffsetAngle, deltaAngle, desiredOffsetAngle);
		}
	}

	float MoveTo(float value, float delta, float target){
		if (target > value) {
			return Mathf.Min (target, value + delta);
		} else {
			return Mathf.Max (target, value - delta);
		}
	}

	public override void HandleStartDestroying ()	{
		base.HandleStartDestroying ();
		helmet.cacheTransform.parent = null;
		helmet.destroyOnBoundsTeleport = true;
		helmet.velocity -= (Vector2)helmet.cacheTransform.right * 10f;
		leftArm.parts.ForEach (p => p.obj.Kill ());
		rightArm.parts.ForEach (p => p.obj.Kill ());
	}

	public class SpawnedArmData {
		public SpawnedJointData shoulder;
		public SpawnedJointData joint;
		public SpawnedJointData hand;
		public MEarthBossData.ArmState state;
		public bool right;

		public List<SpawnedJointData> parts {get { return new List<SpawnedJointData>{ shoulder, joint, hand };}}
	}

	public class SpawnedJointData{
		public PolygonGameObject obj;
		public KeepOrientationEffect angleEff;
		public KeepPositionEffect posEff;

		public void ToggleEffects(bool active) {
			posEff.pause = !active;
			angleEff.pause = !active;
		}

		public void MultiplyEffects(float mul){
			angleEff.MultiplyOriginalTurnSpeed (mul);
			posEff.MultiplyForceAndVelocity (mul);
		}
	}
}