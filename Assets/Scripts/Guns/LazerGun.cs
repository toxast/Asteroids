using UnityEngine;
using System.Collections;

public class LazerGun : Gun
{
	[System.Serializable]
	public class LazerGunData : IClonable<LazerGunData>, IGun
	{
		public string name;
		public GunData baseData;
		public float distance = 50f;
		public float width = 0.5f;

		public string iname{ get {return baseData.name;}}
		public int iprice{ get {return baseData.price;}}
		public GunSetupData.eGuns itype{ get {return GunSetupData.eGuns.LAZER;}}

		public LazerGunData Clone()
		{
			LazerGunData r = new LazerGunData ();
			r.baseData = baseData.Clone ();
			r.distance = distance;
			r.width = width; 
			return r;
		}
	}

	GameObject lazer;
	Mesh lazerMesh;
	Transform lTransform;
	float distance;
	float width;
	float attackDuration;
	float pauseDuration;
	float damage;
	Color color;
	ParticleSystem fireEffect;

	float attackLeftDuration = 0;
	float timeToNextShot;

	int layer = 0;

	public LazerGun(Place place, MLazerGunData data, IPolygonGameObject parent):base(place, data, parent)
	{
		distance = data.distance;
		width = data.width;
		//bulletSpeed = Mathf.Infinity;
		color = data.color;
		attackDuration = data.attackDuration;
		pauseDuration = data.pauseDuration;

		if(data.fireEffect != null)
		{
			fireEffect = GameObject.Instantiate(data.fireEffect) as ParticleSystem;
			Math2d.PositionOnParent(fireEffect.transform, place, parent.cacheTransform, true, -1);
		}

		fireEffect = data.fireEffect;
		damage = data.damage;
	}

	public override float BulletSpeedForAim{ get { return Mathf.Infinity; } }

	public override float Range
	{
		get{return distance;}
	}

	public override bool ReadyToShoot()
	{
		return timeToNextShot <= 0;
	}

	public override void ShootIfReady()
	{
		if(ReadyToShoot())
		{
			CreateLazerGo();
			ResetTime();
		}
	}

	public override void ResetTime()
	{
		timeToNextShot = pauseDuration;
		attackLeftDuration = attackDuration;
	}

	protected void CreateLazerGo ()
	{
		if(lazer == null)
		{
			lazer = PolygonCreator.CreateLazerGO(color);
			lTransform = lazer.transform;
			lazerMesh = lazer.GetComponent<MeshFilter>().mesh;
			Math2d.PositionOnParent (lTransform, place, parent.cacheTransform, true);
			layer = 1 << CollisionLayers.GetBulletLayerNum(parent.layer);
		}
	}

	public override void Tick (float delta)
	{
		if(lazer != null)
			lazer.SetActive (attackLeftDuration > 0);
		
		if (attackLeftDuration > 0) 
		{
			attackLeftDuration -= delta;
			Fire (delta);
		} 
		else if(timeToNextShot > 0)
		{
			timeToNextShot -= delta;
		}
	}

	private void Fire(float delta)
	{
		Vector2 lpos = (Vector2)lTransform.position;
		Vector2 lazerDir = lTransform.right;
		Vector2 perp = lTransform.up;
		var lazerEdge = new Edge (lpos, lpos + lazerDir * distance);
		
		IPolygonGameObject hitObject = null;
		Vector2 hitPlace = Vector2.zero;
		float hitDistance = distance;
		
		var objs = Singleton<Main>.inst.gObjects;
		for (int i = 0; i < objs.Count; i++) 
		{
			var obj = objs[i];
			if(parent == obj)
				continue;

			if((layer & obj.collision) == 0)
				continue;
			
			var dir = obj.position - lpos;
			
			var rDist = Mathf.Abs(Vector2.Dot(dir, perp));
			if(rDist > obj.polygon.R)
				continue;
			
			if(dir.magnitude > distance + obj.polygon.R)
				continue;
			
//			if(obj.globalPolygon == null)
//			{
//				obj.globalPolygon = PolygonCollision.GetPolygonInGlobalCoordinates(obj);
//			}
			var intersections = Intersection.GetIntersections(lazerEdge, obj.globalPolygon.edges);
			foreach(var isc in intersections)
			{
				if(isc.haveIntersection)
				{
					var iDir = isc.intersection - lpos;
					float idist = Vector2.Dot(iDir, lazerDir);
					if(idist < hitDistance)
					{
						hitDistance = idist;
						hitObject = obj;
						hitPlace = isc.intersection;
					}
				}
			}
		}
		
		if(hitObject != null)
		{
			if(hitDistance <= distance)
			{
				hitObject.Hit(damage*delta);
				if (fireEffect != null)
				{
					fireEffect.transform.position = hitPlace;
					fireEffect.transform.right = -lazerDir;
					fireEffect.Emit (1);
				}
			}
		}

		PolygonCreator.ChangeLazerMesh (lazerMesh, hitDistance, width);
	}


}
