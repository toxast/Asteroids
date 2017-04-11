using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ExtraGunsEffect : DurationEffect {
	protected override eType etype { get { return eType.ExtraGuns; } }
	public override bool CanBeUpdatedWithSameEffect { get { return false; } }

	Data data;
	List<Gun> guns;

	public ExtraGunsEffect(Data data) : base(data) {
		this.data = data;
	}

	public override void SetHolder (PolygonGameObject holder) {
		base.SetHolder (holder);
		guns = new List<Gun> ();
		foreach (var gunplace in data.guns) {
			Vector2 hitPos = Vector2.zero;
			Vector2 ray = gunplace.place.pos;
			if (ray == Vector2.zero) {
				ray = new Vector2 (1, 0);
			}
			ray.Normalize ();
			if (!FindFurthestIntersectionPoint (ray, out hitPos) && !FindFurthestIntersectionPoint (-ray, out hitPos)) {
				Debug.LogError ("wtf hit pos");
				hitPos = Vector2.zero;
			}
			Debug.LogWarning ("extra gun pos " + hitPos);
			var gun = gunplace.gun.GetGun (new Place (hitPos, gunplace.place.dir), holder);
			guns.Add (gun);
		}
		holder.AddExtraGuns (guns);
	}

	private bool FindFurthestIntersectionPoint(Vector2 ray, out Vector2 hitPos){
		hitPos = Vector2.zero;
		Edge e = new Edge (Vector2.zero, 100f * ray.normalized);
		var intersections = Intersection.GetIntersections(e, holder.polygon.edges).FindAll(i => i.haveIntersection);
		intersections.Sort ((b, a) => a.intersection.sqrMagnitude.CompareTo (b.intersection.sqrMagnitude));
		if (intersections.Count > 0) {
			hitPos = intersections [0].intersection;
			hitPos = hitPos.normalized * (hitPos.magnitude - 0.1f);
			return true;
		} 
		return false;
	}

    public override void OnExpired() {
        holder.RemoveGuns(guns);
    }

	[System.Serializable]
	public class Data : IHasDuration, IApplyable {
		public float duration;
		public float iduration{get {return duration;} set{duration = value;}}
		public List<MGunSetupData> guns;

		public IHasProgress Apply(PolygonGameObject picker) {
			var effect = new ExtraGunsEffect (this);
			effect = picker.AddEffect (effect);
			return effect;
		}
	}
}


