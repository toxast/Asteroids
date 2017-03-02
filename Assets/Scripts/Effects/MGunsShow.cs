using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MGunsShow : MSpawnDataBase {
	public float duration = 3f;
	public float rotation = 0f;
	public bool makeChild = false; //will inherit rotation
	public List<ElementPositioning> elements;

	//to test in editor mode
	public override PolygonGameObject Create(int layer) {
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


//TODO: separate class
public class GunsShowPolygonGO : PolygonGameObject
{
	List<PolygonGameObject> gunsObjects;
	public void InitGunsShowPolygonGO(List<PolygonGameObject> gunsObjects){
		this.gunsObjects = new List<PolygonGameObject> (gunsObjects);
	}

	public override void Tick (float delta)
	{
		base.Tick (delta);
		for (int i = 0; i < gunsObjects.Count; i++) {
			var obj = gunsObjects [i];
			obj.Tick (delta);
			obj.TickGuns(delta);
			obj.Shoot();
		}
	}
}


public class GunsShowEffect : TickableEffect {
	MGunsShow data;
	PolygonGameObject gunsShowObj;

	protected override eType etype { get { return eType.GunsShow; } }
	public override bool CanBeUpdatedWithSameEffect { get { return false; } }

	public GunsShowEffect(MGunsShow data) {
		this.data = data;
	}

	public override void SetHolder(PolygonGameObject holder) {
		base.SetHolder(holder);
		gunsShowObj = data.CreateObj(holder.layerNum);
		SetPosition ();
		if (data.makeChild) {
			gunsShowObj.cacheTransform.parent = holder.cacheTransform;
			gunsShowObj.cacheTransform.localRotation = Quaternion.identity; 
		}
	}

	void SetPosition(){
		gunsShowObj.position = holder.position;
	}

	public override void HandleHolderDestroying ()
	{
		base.HandleHolderDestroying ();
		DestroyGunsShowObject ();
	}

	public override void Tick(float delta) {
		base.Tick(delta);
		if (!IsFinished()) {
			gunsShowObj.Tick(delta);
			if (!data.makeChild) {
				SetPosition ();
			}
			if (IsFinished ()) {
				DestroyGunsShowObject ();
			}
		} 
	}

	private void DestroyGunsShowObject(){
		if (!Main.IsNull(gunsShowObj)) {
			GameObject.Destroy(gunsShowObj.gameObj);
			gunsShowObj = null;
		}
	}

	public override bool IsFinished() {
		return gunsShowObj == null || gunsShowObj.Expired();
	}

	public void ForceFinish() {
		DestroyGunsShowObject ();
	}
}
