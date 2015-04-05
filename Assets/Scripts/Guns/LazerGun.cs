using UnityEngine;
using System.Collections;

public class LazerGun : Gun
{
	[System.Serializable]
	public class LazerGunData : IClonable<LazerGunData>
	{
		public GunData baseData;
		public float distance = 50f;
		public float width = 0.5f;

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

	float leftDuration = 0;

	int layer = 0;

	public LazerGun(Place place, LazerGunData data, IPolygonGameObject parent):base(place, data.baseData, parent)
	{
		distance = data.distance;
		width = data.width;
		bulletSpeed = Mathf.Infinity;
	}

	public override float Range
	{
		get{return distance;}
	}

	protected override IBullet CreateBullet ()
	{
		if(lazer == null)
		{
			lazer = PolygonCreator.CreateLazerGO(color);
			lTransform = lazer.transform;
			lazerMesh = lazer.GetComponent<MeshFilter>().mesh;
			Math2d.PositionOnShooterPlace (lTransform, place, parent.cacheTransform, true);
			layer = 1 << Main.GetBulletLayerNum(parent.layer);
		}
		leftDuration = lifeTime;
		return null;
	}

	public override void Tick (float delta)
	{
		base.Tick (delta);

		if(lazer != null)
			lazer.SetActive (leftDuration > 0);
		
		if(leftDuration > 0)
		{
			leftDuration -= delta;
			Fire(delta);
		}
	}

	protected override void Fire (IBullet b){}

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
