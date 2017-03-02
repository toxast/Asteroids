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
	Renderer lazerRenderer;
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

	float ApplyHeavvyBulletInterval = 0.2f; //created interval to spaceships wouldn't instantly absord small delta-rotations
	float timeLeftToApplyHeavvyBullet = 0;

	float lazerAppearDuration = 0.25f;
	float appearTimeLfet = 0;

	public LazerGun(Place place, MLazerGunData data, PolygonGameObject parent):base(place, data, parent)
	{
		distance = data.distance;
		width = data.width;
		//bulletSpeed = Mathf.Infinity;
		color = data.color;
		attackDuration = data.attackDuration;
		pauseDuration = data.pauseDuration;
		appearTimeLfet = lazerAppearDuration;

		if(data.fireEffect != null)
		{
			fireEffect = GameObject.Instantiate(data.fireEffect) as ParticleSystem;
			Math2d.PositionOnParent(fireEffect.transform, place, parent.cacheTransform, true, -1);
		}

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
			SetTimeForNextShot();
		}
	}

	public override void SetTimeForNextShot()
	{
		timeToNextShot = pauseDuration;
		attackLeftDuration = attackDuration;
	}

	protected void CreateLazerGo ()
	{
		if(lazer == null)
		{
			lazer = PolygonCreator.CreateLazerGO(color);
			lazerRenderer = lazer.GetComponent<Renderer> ();
			lTransform = lazer.transform;
			lazerMesh = lazer.GetComponent<MeshFilter>().mesh;
			Math2d.PositionOnParent (lTransform, place, parent.cacheTransform, true);
			layer = 1 << CollisionLayers.GetBulletLayerNum(parent.layerLogic);
			PolygonCreator.ChangeLazerMesh (lazerMesh, distance, width);
			lazerRenderer.material.SetFloat("_Alpha", 0);
		}
	}

	public override void OnGunRemoving ()
	{
		base.OnGunRemoving ();
		if (lazer != null) {
			GameObject.Destroy (lazer.gameObject);
			lazer = null;
		}
		if (fireEffect != null) {
			GameObject.Destroy (fireEffect.gameObject);
			fireEffect = null;
		}
	}

	public override void Tick (float delta) {
		if (lazer != null) {
			lazer.SetActive (attackLeftDuration > 0);
		}
		if (attackLeftDuration > 0) {
			attackLeftDuration -= delta;
			appearTimeLfet -= delta;
			Fire (delta);
		} else if (timeToNextShot > 0) {
			timeToNextShot -= delta;
			appearTimeLfet = lazerAppearDuration;
		}
	}

	public bool IsFiring(){
		return attackLeftDuration > 0;
	}

	private void Fire(float delta)
	{
		Vector2 lpos = (Vector2)lTransform.position;
		Vector2 lazerDir = lTransform.right;
		Vector2 perp = lTransform.up;
		var lazerEdge = new Edge (lpos, lpos + lazerDir * distance);
		
		PolygonGameObject hitObject = null;
		Vector2 hitPlace = Vector2.zero;
		Edge hitEdge = new Edge ();
		float hitDistance = distance;
		
		var objs = Singleton<Main>.inst.gObjects;
		for (int i = 0; i < objs.Count; i++) 
		{
			var obj = objs[i];
			if(parent == obj)
				continue;

			if((layer & obj.collisions) == 0)
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
						hitEdge = isc.edge2;
					}
				}
			}
		}
		
		if(hitObject != null)
		{
			if(hitDistance <= distance)
			{
				hitObject.Hit(damage*delta);

				//heavvy bullet logic

				if (parent.heavyBulletData != null){ 
					timeLeftToApplyHeavvyBullet -= delta;
					if (timeLeftToApplyHeavvyBullet <= 0) {
						timeLeftToApplyHeavvyBullet += ApplyHeavvyBulletInterval;
						Vector2 egde = hitEdge.p2 - hitEdge.p1;
						Vector2 egdeRight = Math2d.MakeRight (egde).normalized;
						float force = 30f * damage * Vector2.Dot (lazerDir, egdeRight) * parent.heavyBulletData.multiplier * ApplyHeavvyBulletInterval;
						PolygonCollision.ApplyForce (hitObject, hitPlace, force * egdeRight);
					}
				}

				if (fireEffect != null)
				{
					fireEffect.transform.position = hitPlace;
					fireEffect.transform.right = -lazerDir;
					fireEffect.Emit (1);
				}
			}
		}
		lazerRenderer.material.SetFloat("_HitCutout", hitDistance/distance);

		float alpha = 1;
		if (appearTimeLfet >= 0) {
			alpha = 1f - Mathf.Clamp01 (appearTimeLfet / lazerAppearDuration);
		} else if(attackLeftDuration <= lazerAppearDuration) {
			alpha =  Mathf.Clamp01 (attackLeftDuration / lazerAppearDuration);
		}
		lazerRenderer.material.SetFloat("_Alpha", alpha);
	}


}
