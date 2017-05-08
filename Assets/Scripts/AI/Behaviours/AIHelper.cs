using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public static class AIHelper
{
	public class Data
	{
		public Vector2 dir;
		public float distCenter2Center;
		public float distEdge2Edge;
		public Vector2 dirNorm;
		public float vprojThis;
		public float vprojTarget;
		public float evadeSign;
		
		public void Refresh(PolygonGameObject thisShip, PolygonGameObject target)
		{
            if (Main.IsNull(thisShip) || Main.IsNull(target))
                return;

			dir = target.position - thisShip.position;
			distCenter2Center = dir.magnitude;
			if (thisShip.polygon.R > 10 || target.polygon.R > 10) {
				distEdge2Edge = GetDistanceBetweenObjectVerts (thisShip, target);
			} else {
				distEdge2Edge = distCenter2Center - (thisShip.polygon.R + target.polygon.R);
			}
			dirNorm = dir/distCenter2Center;
			vprojThis = Vector2.Dot(thisShip.velocity, dirNorm); //+ means towards other ship
			vprojTarget = Vector2.Dot(target.velocity, -dirNorm); //+ means towards other ship
			evadeSign = Mathf.Sign(Math2d.Cross2(target.velocity - thisShip.velocity, dir));
		}
	}

	public static float GetDistanceBetweenObjectVerts(PolygonGameObject a, PolygonGameObject b){
		PolygonGameObject big = (a.polygon.R > b.polygon.R) ? a : b;
		PolygonGameObject small = (big == a) ? b : a;

		Vector2 closetsVert = big.position;
		float closestDistSqr = float.MaxValue;
		foreach(var vert in big.globalPolygon.vertices)
		{
			var distSqr = (vert - small.position).sqrMagnitude;
			if(distSqr < closestDistSqr) {
				closestDistSqr = distSqr;
				closetsVert = vert;
			}
		}
		return Mathf.Sqrt (closestDistSqr) - small.polygon.R;
	}


	public static bool EvadeTarget(SpaceShip ship, PolygonGameObject target, Data tickData, out float duration, out Vector2 newDir)
	{ 
		newDir = Vector2.zero;
		duration = 0f;
		if(target.mass > ship.mass*0.8f)
		{
			if(tickData.distEdge2Edge < 1.3f*(tickData.vprojThis + tickData.vprojTarget))
			{
				float angle = UnityEngine.Random.Range(40, 110);
				newDir = Math2d.RotateVertexDeg(tickData.dirNorm, tickData.evadeSign * angle);
				duration = UnityEngine.Random.Range(-0.3f, 0.4f) + (angle / ship.turnSpeed) + ((ship.polygon.R + target.polygon.R) * 2f) / (ship.originalMaxSpeed * 0.8f);// UnityEngine.Random.Range(0.5f, 1.5f);
				return true;
			}
		}
		return false;
	}

	public static void OutOfComformTurn(SpaceShip ship, float comformDistanceMax, float comformDistanceMin, Data tickData, out float duration, out Vector2 newDir)
	{
		float angle = 0;
		if(tickData.distEdge2Edge > comformDistanceMax)
		{
			////Debug.LogWarning("far");
			angle = UnityEngine.Random.Range(-30, 30);
		}
		else
		{
			////Debug.LogWarning("close");
			angle = -tickData.evadeSign * UnityEngine.Random.Range(80, 100);
		}
		float restoreDist =  UnityEngine.Random.Range(20f, 36f);
		duration =  restoreDist / ship.originalMaxSpeed;
		
		var dirFromTarget =  Math2d.RotateVertexDeg(-tickData.dirNorm, angle);  
		dirFromTarget *= (comformDistanceMax + comformDistanceMin)/2f;
		dirFromTarget *= UnityEngine.Random.Range(0.8f, 1.2f);
		newDir =  tickData.dir + dirFromTarget;
	}
	
	public static void ComfortTurn(float comformDistanceMax, float comformDistanceMin, Data tickData, out float duration, out Vector2 newDir)
	{
		if (tickData.distEdge2Edge > (comformDistanceMax + comformDistanceMin) / 2f) 
		{
			float angle = UnityEngine.Random.Range (30, 80) * Math2d.RandomSign();
			newDir = Math2d.RotateVertexDeg(tickData.dirNorm, angle);
		} 
		else
		{
			float angle = UnityEngine.Random.Range (80, 110);
			newDir = Math2d.RotateVertexDeg(tickData.dirNorm, Math2d.RandomSign() * angle);
		}
		duration =  UnityEngine.Random.Range(0.8f, 1.5f);
	}

	public static Vector2 RotateDirection(Vector2 dir, float angleMin, float angleMax)
	{
		float angle = UnityEngine.Random.Range (angleMin, angleMax) * Math2d.RandomSign();
		return Math2d.RotateVertexDeg(dir, angle);
	}

	public static bool BulletDanger(PolygonGameObject go, PolygonGameObject b)
	{
		if (b == null)
			return false;
		
		if((b.collisions & go.layerCollision) == 0)
			return false;
		
		Vector2 dir2thisShip = go.position - b.position;
		
		float angleVS = Math2d.AngleRad (b.velocity, go.velocity);
		float cosVS = Mathf.Cos (angleVS);
		if(cosVS < -0.9f)
		{
			var cos = Mathf.Cos(Math2d.AngleRad (b.velocity, dir2thisShip));
			return cos  > 0.9f;
		}
		
		return false;
	}

	public static bool EvadeBullets(SpaceShip ship, List<PolygonGameObject> bullets, out float duration, out Vector2 newDir)
	{
		newDir = Vector2.zero;
		duration = 0f;
		foreach (var b in bullets)
		{
			if (AIHelper.BulletDanger(ship, b))
			{
				var bulletDir = b.position - ship.position;
				float bulletEvadeSign = Mathf.Sign(Math2d.Cross2(b.velocity, bulletDir));
				float angle = 90;
				newDir = Math2d.RotateVertexDeg(bulletDir.normalized, bulletEvadeSign * angle);
				duration = (angle / ship.originalTurnSpeed) + ((ship.polygon.R + b.polygon.R) * 2f) / (ship.originalMaxSpeed / 2f);
				return true;
			}
		}
		return false;
	}

	public static IEnumerator TimerR(float time, Func<float> deltaTime, Action act){
		AIHelper.MyTimer timer = new AIHelper.MyTimer (time, null);
		while (!timer.IsFinished ()) {
			timer.Tick (deltaTime());
			act ();
			yield return null;
		}
	}

	public static IEnumerator TimerR(float time, Func<float> deltaTime){
		AIHelper.MyTimer timer = new AIHelper.MyTimer (time, null);
		while (!timer.IsFinished ()) {
			timer.Tick (deltaTime());
			yield return null;
		}
	}
	
	public static void ChangeAccuracy(ref float accuracy, ref Vector2 lastDir, PolygonGameObject target, AccuracyData data)
	{
		float sameVelocityMesure = 0;
		if(Math2d.ApproximatelySame(target.velocity, Vector2.zero) || Math2d.ApproximatelySame(lastDir, Vector2.zero))
		{
			sameVelocityMesure = data.add;
		}
		else
		{
			var cos =  Math2d.Cos(target.velocity, lastDir); 
			sameVelocityMesure = (cos > 0.9) ? data.add : -data.sub; //TODO: 0.9?
		}
		accuracy += sameVelocityMesure*data.checkDtime;
		accuracy = Mathf.Clamp(accuracy, data.bounds.x, data.bounds.y);
		lastDir = target.velocity;
	}

	public class AccuracyChangerAdvanced : ITickable
	{
		public float accuracy{ private set; get;}

		PolygonGameObject parent;
		AccuracyData data;
		float dtime;

		MyRepeatTimer timer;
		bool hasEstimatedPosition = false;
		Vector2 estimatedPosition = Vector2.zero;
		PolygonGameObject lastTarget = null;

		public AccuracyChangerAdvanced(AccuracyData data, PolygonGameObject parent) {
			this.parent = parent;
			this.data = data;
			this.dtime = data.checkDtime;
			timer = new MyRepeatTimer(dtime, ChangeAccuracy);
			accuracy = data.startingAccuracy;
		}

		public void Tick(float delta) {
			if (data.isDynamic) {
				timer.Tick (delta);
			}
		}

		void ChangeAccuracy() {
			var target = parent.target;
			if (lastTarget != target) {
				hasEstimatedPosition = false;
				accuracy = data.startingAccuracy;
			}

			if (!Main.IsNull (target)) {
				if (!hasEstimatedPosition) {
					estimatedPosition = target.position + target.velocity * dtime;
					hasEstimatedPosition = true;
				} else {
					float diffDistance = (target.position - estimatedPosition).magnitude;

					if (diffDistance >= data.thresholdDistance) {
						accuracy -= dtime * data.sub;
					} else {
						accuracy += dtime * data.add;
					}
					accuracy = Mathf.Clamp (accuracy, data.bounds.x, data.bounds.y);
					//Debug.LogError ("diffDistance " + diffDistance + " accuracy " + accuracy);

					estimatedPosition = target.position + target.velocity * dtime;
				}
			}
			lastTarget = target;
		}
	}


	public class MyRepeatTimer : ITickable {
		float timeLeft;
		float repeatInterval;
		Action act;
		public MyRepeatTimer(float repeatInterval, Action act) {
			this.repeatInterval = repeatInterval;
			this.act = act;
			timeLeft = repeatInterval;
		}

		public void Tick(float delta) {
			timeLeft -= delta;
			if (timeLeft <= 0) {
				timeLeft += repeatInterval;
				act ();
			}
		}
	}

	public class MyTimer : ITickable {
		float timeLeft;
		//float duration;
		Action act;
		public MyTimer(float duration, Action act) {
			//this.duration = duration;
			this.act = act;
			Reset(duration);
		}

		public bool IsFinished(){
			return timeLeft <= 0;
		}

		public void Tick(float delta) {
			if (timeLeft > 0) {
				timeLeft -= delta;
				if (timeLeft <= 0) {
					if(act != null) act ();
				}
			}
		}

		public void Reset(float duration){
			timeLeft = duration;
		}
	}

}

public enum AIType
{
	eCommon = 0,
	eSuicide = 1,
}
