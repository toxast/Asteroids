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
		
		public void Refresh(IPolygonGameObject thisShip, IPolygonGameObject target)
		{
			dir = target.position - thisShip.position;
			distCenter2Center = dir.magnitude;
			distEdge2Edge = distCenter2Center - (thisShip.polygon.R + target.polygon.R);
			dirNorm = dir/distCenter2Center;
			vprojThis = Vector2.Dot(thisShip.velocity, dirNorm); //+ means towards other ship
			vprojTarget = Vector2.Dot(target.velocity, -dirNorm); //+ means towards other ship
			evadeSign = Mathf.Sign(Math2d.Cross2(target.velocity - thisShip.velocity, dir));
		}
	}


	public static bool EvadeTarget(SpaceShip ship, IPolygonGameObject target, Data tickData, out float duration, out Vector2 newDir)
	{ 
		newDir = Vector2.zero;
		duration = 0f;
		if(target.mass > ship.mass*0.8f)
		{
			if(tickData.distEdge2Edge < 1.3f*(tickData.vprojThis + tickData.vprojTarget))
			{
				float angle = UnityEngine.Random.Range(90-15, 90+25);
				newDir = Math2d.RotateVertex(tickData.dirNorm, tickData.evadeSign * angle * Mathf.Deg2Rad);
				duration = (angle / ship.turnSpeed) + ((ship.polygon.R + target.polygon.R) * 2f) / (ship.maxSpeed * 0.8f);// UnityEngine.Random.Range(0.5f, 1.5f);
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
		float restoreDist =  UnityEngine.Random.Range(30f, 40f);
		duration =  restoreDist / ship.maxSpeed;
		
		var dirFromTarget =  Math2d.RotateVertex(-tickData.dirNorm, angle * Mathf.Deg2Rad);  
		dirFromTarget *= (comformDistanceMax + comformDistanceMin)/2f;
		dirFromTarget *= UnityEngine.Random.Range(0.8f, 1.2f);
		newDir =  tickData.dir + dirFromTarget;
	}
	
	public static void ComfortTurn(float comformDistanceMax, float comformDistanceMin, Data tickData, out float duration, out Vector2 newDir)
	{
		if (tickData.distEdge2Edge > (comformDistanceMax + comformDistanceMin) / 2f) 
		{
			float angle = UnityEngine.Random.Range (30, 80) * Mathf.Sign (UnityEngine.Random.Range (-1f, 1f));
			newDir = Math2d.RotateVertex (tickData.dirNorm, angle * Mathf.Deg2Rad);
		} 
		else
		{
			float angle = UnityEngine.Random.Range (80, 110);
			newDir = Math2d.RotateVertex (tickData.dirNorm, tickData.evadeSign * angle * Mathf.Deg2Rad);
		}
		duration =  1f;
	}

	public static Vector2 RotateDirection(Vector2 dir, float angleMin, float angleMax)
	{
		float angle = UnityEngine.Random.Range (angleMin, angleMax) * Mathf.Sign (UnityEngine.Random.Range (-1f, 1f));
		return Math2d.RotateVertex(dir, angle*Mathf.Deg2Rad);
	}

	public static bool BulletDanger(IPolygonGameObject go, IBullet b)
	{
		if (b == null)
			return false;
		
		if((b.collision & go.layer) == 0)
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

	public static bool EvadeBullets(SpaceShip ship, List<IBullet> bullets, out float duration, out Vector2 newDir)
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
				newDir = Math2d.RotateVertex(bulletDir.normalized, bulletEvadeSign * angle * Mathf.Deg2Rad);
				duration = (angle / ship.turnSpeed) + ((ship.polygon.R + b.polygon.R) * 2f) / (ship.maxSpeed / 2f);
				return true;
			}
		}
		return false;
	}

	
	public static void ChangeAccuracy(ref float accuracy, ref Vector2 lastDir, IPolygonGameObject target, AccuracyData data)
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

}
