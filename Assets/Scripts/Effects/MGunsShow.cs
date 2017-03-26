using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MGunsShow : MSpawnDataBase , IApplyable, IHasDuration {
	public float duration = 3f;
	public float rotation = 0f;
	public bool makeChild = false; //will inherit rotation
	public List<ElementPositioning> elements;

    public float iduration { get { return duration; } set { duration = value; } }

    //to test in editor mode
    protected override PolygonGameObject CreateInternal(int layer) {
		if (Singleton<Main>.inst.userSpaceship != null) {
			Singleton<Main>.inst.userSpaceship.AddEffect(new GunsShowEffect(this));
			return null;
		} else {
			var verts = PolygonCreator.CreatePerfectPolygonVertices (3, 6);
			PolygonGameObject holder = PolygonCreator.CreatePolygonGOByMassCenter<PolygonGameObject> (verts, Color.red);
			var ph = new PhysicalData ();
			ph.health = 10000;
			ph.density = 100;
			holder.InitPolygonGameObject (ph);
			holder.SetLayerNum (layer);
			holder.InitLifetime (duration + 1);
			holder.destructionType = PolygonGameObject.DestructionType.eDisappear;
			holder.name = "fake holder";
			return holder;
		}
	}

	public IHasProgress Apply(PolygonGameObject picker) {
		var effect = new GunsShowEffect (this);
		picker.AddEffect (effect);
		return effect;
	}

	[System.Serializable]
	public class ElementPositioning
	{
		public MGunsShowElement element;
		public Place offset;
	}

	public PolygonGameObject CreateObj(int layer){
		var verts1 = PolygonCreator.CreatePerfectPolygonVertices(3, 6);
		GunsShowPolygonGO gunsShowObj = PolygonCreator.CreatePolygonGOByMassCenter<GunsShowPolygonGO>(verts1, Color.red);
		gunsShowObj.SetAlpha (0);
		gunsShowObj.InitPolygonGameObject (new PhysicalData ());
		gunsShowObj.SetLayerNum (layer);
		gunsShowObj.InitLifetime (duration);
		gunsShowObj.rotation = rotation;
		gunsShowObj.name = "guns show";
		List<PolygonGameObject> gunsObjects = new List<PolygonGameObject> ();
		for (int i = 0; i < elements.Count; i++) {
			var verts = PolygonCreator.CreatePerfectPolygonVertices(3, 4);
			var obj = PolygonCreator.CreatePolygonGOByMassCenter<PolygonGameObject>(verts, Color.black);
			obj.SetAlpha(0);
			var elem = elements [i].element;
			var gunsList = new List<Gun>();
			foreach (var gunplace in elem.guns) {
				var gun = gunplace.GetGun(obj);
				gunsList.Add(gun);
			}
			obj.InitPolygonGameObject (new PhysicalData ());
			obj.SetGuns(gunsList, elem.linkedGuns);
			obj.SetLayerNum(layer);
			obj.rotation = elem.rotation;
			obj.name = "guns element";
			Math2d.PositionOnParent (obj.cacheTransform, elements [i].offset, gunsShowObj.cacheTransform, true, 1);
			gunsObjects.Add (obj);
		}
		gunsShowObj.InitGunsShowPolygonGO (gunsObjects);
		return gunsShowObj;
	}
}