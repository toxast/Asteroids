using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Fire1SpaceshipController : BaseSpaceshipController, IGotTarget {

	//TODO better bound teleport check
	MFireShip1Data data;

    //float force;
    //float deltaAngle;
    List<SpaceShip> fireballs = new List<SpaceShip>();
    //float comformDistanceMin = 30f;
    //float comformDistanceMax = 50f;
    //AIHelper.Data tickData = new AIHelper.Data();

	//bool shootBeh = false;

	CommonBeh.Data behData;
	public Fire1SpaceshipController(SpaceShip thisShip, List<PolygonGameObject> objects, MFireShip1Data data) : base(thisShip, data.accuracy) {
        //this.data = data;

		var comformDistanceMax = data.overrideMaxComfortDist > 0 ? data.overrideMaxComfortDist:  data.fireballData.lifeTime * data.fireballData.missleParameters.maxSpeed * 0.7f;
		var comformDistanceMin = comformDistanceMax / 2f;
		behData = new CommonBeh.Data {
			accuracyChanger = accuracyChanger,
			comformDistanceMax = comformDistanceMax,
			comformDistanceMin = comformDistanceMin,
			getTickData = GetTickData,
			mainGun = null,
			thisShip = thisShip,
		};

		thisShip.OnDestroying += HandleDestroying;

		EvadeTargetBeh evadeBeh = new EvadeTargetBeh(behData, new NoDelayFlag());
		logics.Add(evadeBeh);

		ShootFireballBeh shootBeh = new ShootFireballBeh (behData, new NoDelayFlag (), data, fireballs);
		logics.Add(shootBeh);

		TurnBeh turn = new TurnBeh (behData, new NoDelayFlag ());
		turn.SetPassiveTickOthers (true);
		logics.Add (turn);

		FlyAroundBeh noTarget = new FlyAroundBeh (behData);
		logics.Add (noTarget);

		AssignCurrentBeh (null);

		spawnFireballs = new SpawnFireballsBeh (behData, new NoDelayFlag (), data, fireballs, () => shootBeh.IsFinished ());
		if (spawnFireballs.IsReadyToAct ()) {
			spawnFireballs.Start ();
		}

		var force = thisShip.thrust;
		var deltaAngle = 360f / data.fireballCount;
		keepFireballs = new KeepFireballsBeh (behData, new NoDelayFlag (), data, fireballs, force, deltaAngle);
		if (keepFireballs.IsReadyToAct ()) {
			keepFireballs.Start ();
		}
    }

	IBehaviour spawnFireballs;
	IBehaviour keepFireballs;
	public override void Tick (float delta) {
		base.Tick (delta);
		spawnFireballs.Tick (delta);
		keepFireballs.Tick (delta);
	}

	void HandleDestroying() {
		for (int i = 0; i < fireballs.Count; i++) {
			var obj = fireballs[i];
			if (!Main.IsNull(obj)) {
				obj.Kill(PolygonGameObject.KillReason.EXPIRED);
			}
		}
	}

}

public class SpawnFireballsBeh : DelayedActionBeh{

	MFireShip1Data fdata;
	List<SpaceShip> fireballs;
	Func<bool> generateFireBalls;
	public SpawnFireballsBeh (CommonBeh.Data data, IDelayFlag delay, MFireShip1Data fdata, List<SpaceShip> fireballsLink, Func<bool> generateFireBalls):base(data, delay) {
		this.fdata = fdata;
		this.fireballs = fireballsLink;
		this.generateFireBalls = generateFireBalls;
	}

	protected override IEnumerator Action ()
	{
		for (int i = 0; i < fdata.fireballCount; i++) {
			if (i < fdata.startFireballs) {
				var fireball = CreateFireball();
				fireballs.Add(fireball);
			} else {
				fireballs.Add(null);
			}
		}

		while (true) {
			bool hasEmpty = fireballs.Exists(a => a == null);
			if (hasEmpty && generateFireBalls()) {
				var wait = WaitForSeconds (fdata.respawnFireballDuration);
				while (wait.MoveNext()) yield return true;
				int indx = fireballs.FindIndex(a => a == null);
				if (indx >= 0) {
					var obj = CreateFireball();
					fireballs[indx] = obj;
				}
			}
			yield return true;
		}
	}

	SpaceShip CreateFireball() {
		var rd = fdata.fireballData;
		var fireballGun = rd.GetGun (new Place (), thisShip) as FireballGun;
		var fireball = fireballGun.CreateBullet ();
		//hacks
		fireball.velocity = Vector2.zero;
		fireball.SetInfiniteLifeTime ();
		fireball.SetController (new StaticInputController ());
		fireball.destroyOnBoundsTeleport = false;
		thisShip.AddObjectAsFollower(fireball);
		Singleton<Main>.inst.HandleGunFire (fireball);
		return fireball;
	}
}

public class KeepFireballsBeh : DelayedActionBeh {
	MFireShip1Data fdata;
	List<SpaceShip> fireballs;
	float force;
	float deltaAngle;
	public KeepFireballsBeh (CommonBeh.Data data, IDelayFlag delay, MFireShip1Data fdata, List<SpaceShip> fireballsLink, float force, float deltaAngle):base(data, delay) {
		this.fdata = fdata;
		this.fireballs = fireballsLink;
		this.deltaAngle = deltaAngle;
		this.force = force;
	}

	protected override IEnumerator Action ()
	{
		while (true) {
			float angle = 0;
			for (int i = 0; i < fireballs.Count; i++) {
				var item = fireballs[i];
				if (Main.IsNull(item)) {
					fireballs[i] = null;
				} else {
					var radAngle = angle * Mathf.Deg2Rad;
					Vector2 targetPos = thisShip.position + fdata.radius * new Vector2(Mathf.Cos(radAngle), Mathf.Sin(radAngle));
					Vector2 targetVelocity = thisShip.velocity;
					FollowAim aim = new FollowAim(targetPos, targetVelocity, item.position, item.velocity, force, item.maxSpeed);
					item.Accelerate(DeltaTime(), force, item.stability, item.maxSpeed, item.maxSpeedSqr, aim.forceDir.normalized);
				}
				angle += deltaAngle;
			}
			yield return true;
		}
	}
}

public class ShootFireballBeh: DelayedActionBeh {
	MFireShip1Data fdata;
	List<SpaceShip> fireballs;
	public ShootFireballBeh (CommonBeh.Data data, IDelayFlag delay, MFireShip1Data fdata, List<SpaceShip> fireballsLink):base(data, delay) {
		this.fdata = fdata;
		this.fireballs = fireballsLink;
	}

	public override bool IsReadyToAct ()
	{
		if( base.IsReadyToAct () && TargetNotNull) {
			bool fireballsFull = fireballs.Count > 0 && fireballs.TrueForAll (a => !Main.IsNull (a));
			return fireballsFull;
		}
		return false;
	}

	protected override IEnumerator Action ()
	{
		FireBrake ();
		{
			var wait = AIHelper.TimerR (1, DeltaTime, () => FireDirChange (target.position - thisShip.position), () => TargetIsNull);
			while (wait.MoveNext()) yield return true;
		}
		int aimSign = 1;
		for (int i = 0; i < fireballs.Count; i++) {
			if (TargetNotNull) {
				FireDirChange(target.position - thisShip.position);
				var obj = fireballs [i];
				Vector2 aimPos = target.position;
				Vector2 aimVel = target.velocity;
				if (!Main.IsNull (obj)) {
					fireballs [i] = null;
					if (!fdata.fixAim) {
						aimPos = target.position;
						aimVel = target.velocity;
					}
					AimSystem aim = new AimSystem (aimPos, aimVel, obj.position, (fdata.fireballData.velocity + fdata.fireballData.missleParameters.maxSpeed) * 0.5f);
					var controller = new MissileController (obj, fdata.fireballData.accuracy);
					obj.SetController (controller);
					obj.SetTarget (target);
					float rndAngle = aimSign * fdata.randomizeAimAngle; 
					aimSign = -aimSign;
					var dir = aim.directionDist.normalized;
					dir = Math2d.RotateVertexDeg (dir, rndAngle);
					obj.cacheTransform.right = dir; 
					obj.velocity = obj.cacheTransform.right * fdata.fireballData.velocity;
					obj.InitLifetime (fdata.fireballData.lifeTime); 
					obj.destroyOnBoundsTeleport = true;
					thisShip.RemoveFollower (obj);
				}

				var wait = WaitForSeconds(fdata.shootInterval);
				while (wait.MoveNext()) yield return true;
			}
		}
	}
}
	