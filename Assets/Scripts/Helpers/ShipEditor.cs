using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class ShipEditor : MonoBehaviour 
{
	public enum eSaveLoadType
	{
		SpaceShip,
		Turret,
		Tower,
		BulletGun,
		RocketGun,
	}

	Mesh mesh;
	Vector3 vertPos;
	[SerializeField] List<GameObject> handles;
	[SerializeField] List<GameObject> guns;
	[SerializeField] List<GameObject> thrusters;
	[SerializeField] List<GameObject> turrets;

	[SerializeField] int duplicateIndx = 0;
	[SerializeField] bool duplicate = false;
	[SerializeField] bool reverse = false;
    [SerializeField] bool symmetric = true;

    [SerializeField] bool scaleUp = false;
	[SerializeField] bool scaleDown = false;
	[SerializeField] bool doScale = false;
	[SerializeField] Vector2 scaleBy = Vector2.one;
//	[SerializeField] int saveANDloadIndx = -1;
//	[SerializeField] eSaveLoadType editType;

	Vector2 vright = new Vector2(1,0);

    bool symmetricState = true;
    [SerializeField] MonoBehaviour prefab;

	void OnEnable()
	{
        Clear ();

        if (Application.isPlaying) {
            Debug.LogWarning ("editor should be closed at runtime");
            gameObject.SetActive (false);
            return;
        }

		duplicateIndx = 0;
		duplicate = false;
		
		mesh = GetComponent<MeshFilter>().sharedMesh;
		var verts = mesh.vertices;
        if(prefab != null)
		{
            //			object obj = GetObjByType(editType, saveANDloadIndx);

            var shape = prefab as IGotShape;
            verts = shape.iverts.ToList().ConvertAll( v => (Vector3)v).ToArray();

            symmetric = symmetricState = CheckIsSymmetric(verts);

            var oIGotThrusters = prefab as IGotThrusters;
            if (oIGotThrusters != null) {
                var dataThrusters = oIGotThrusters.ithrusters.ConvertAll (t => t.place);
                for (int i = 0; i < dataThrusters.Count; i++) {
                    CreatePosition (dataThrusters [i].pos, dataThrusters [i].dir, "truster ", thrusters);
                }
            }

            var oIGotGuns = prefab as IGotGuns;
            if (oIGotGuns != null) {
                var dataGuns = oIGotGuns.iguns.ConvertAll (t => t.place);
                for (int i = 0; i < dataGuns.Count; i++) {
                    CreatePosition (dataGuns [i].pos, dataGuns [i].dir, "gun ", guns);
                }
            }

            var oIGotTurrets = prefab as IGotTurrets;
            if (oIGotTurrets != null) {
                var dataGuns = oIGotTurrets.iturrets.ConvertAll (t => t.place);
                for (int i = 0; i < dataGuns.Count; i++) {
                    CreatePosition (dataGuns [i].pos, dataGuns [i].dir, "turret ", turrets);
                }
            }
		}

		foreach(var vert in verts)
		{
			if(vert.y <= 0)
			{
				CreatePosition(vert,  vright, "handle ", handles);
			}
		}
	}

//	[ContextMenu ("Custom init action")]
//	private void CustomInitAction()
//	{
//		mesh = GetComponent<MeshFilter>().sharedMesh;
//		var verts = mesh.vertices;
//		verts = MGunsResources.Instance.guns[6].vertices.ToList().ConvertAll( v => (Vector3)v).ToArray();
//		foreach(var vert in verts)
//		{
//			if(vert.y <= 0)
//			{
//				CreatePosition(vert,  vright, "handle ", handles);
//			}
//		}
//	}
//
//	[ContextMenu ("Custom save action")]
//	private void CustomSaveAction()
//	{
//		Vector2[] v2 = new Vector2[handles.Count];    
//		for(int i = 0; i < handles.Count; i++)
//		{
//			v2[i] = handles[i].transform.localPosition;    
//		}
//		
//		var verts = PolygonCreator.GetCompleteVertexes (v2, 1).ToArray();
//		var pivot = Math2d.GetMassCenter (verts);
//		Math2d.ShiftVertices(verts, -pivot);
//		MGunsResources.Instance.guns[6].vertices = verts;
//
//		#if UNITY_EDITOR
//		EditorUtility.SetDirty (MGunsResources.Instance);
//		#endif
//	}

	[ContextMenu ("Create Gun Position")]
	private void CreateGunPosition()
	{
		CreatePosition (Vector3.zero, vright, "gun ", guns);
	}
	
	[ContextMenu ("Create Thruster Position")]
	private void CreateThrusterPosition()
	{
		CreatePosition (Vector3.zero, vright, "thruster ", thrusters);
	}

	[ContextMenu ("Create Turret Position")]
	private void CreateTurretPosition()
	{
		CreatePosition (Vector3.zero, vright, "turret ", turrets);
	}

	[ContextMenu ("Reset")]
	void Reset () {
		handles.ForEach (h => { h.transform.localPosition = Vector3.zero;});
	}

	private void GetCurrentData(out Vector2[] fullArray, 
	                            out List<MGunSetupData> gunsData,
	                            out List<ThrusterSetupData> thrustersData,
	                            out List<MTurretReferenceData> turretsData)
	{
		fullArray = new Vector2[1];
        gunsData = new List<MGunSetupData> ();
		thrustersData = new List<ThrusterSetupData> ();
        turretsData = new List<MTurretReferenceData> ();

		Vector2[] v2 = new Vector2[handles.Count];    
		for(int i = 0; i < handles.Count; i++)
		{
			v2[i] = handles[i].transform.localPosition;    
		}
		
		var full = symmetricState ? PolygonCreator.GetCompleteVertexes (v2, 1) : new List<Vector2>(v2);
		if(reverse)
		{
			full.Reverse();
		}
		
		fullArray = full.ToArray ();
		var pivot = Math2d.GetMassCenter (fullArray);
		Math2d.ShiftVertices(fullArray, -pivot);

		for (int i = 0; i < fullArray.Length; i++) {
			if(Mathf.Abs(fullArray[i].y) < 0.01f)
			{
				fullArray[i].y = 0;
			}
		}

		gunsData = new List<MGunSetupData> ();
		foreach(var g in guns)
		{
            MGunSetupData gd = new MGunSetupData();
			gd.place = new Place((Vector2)g.transform.localPosition - pivot, g.transform.right);
			gunsData.Add(gd);
		}
		
		thrustersData = new List<ThrusterSetupData> ();
		foreach(var t in thrusters)
		{
			ThrusterSetupData td = new ThrusterSetupData();
			td.place = new Place((Vector2)t.transform.localPosition - pivot, t.transform.right);
			thrustersData.Add(td);
		}
		
		turretsData = new List<MTurretReferenceData> ();
		foreach(var t in turrets)
		{
            MTurretReferenceData td = new MTurretReferenceData();
			td.place = new Place((Vector2)t.transform.localPosition - pivot, t.transform.right);
			turretsData.Add(td);
		}
	}

//	[ContextMenu ("Insert")]
//	private void Insert()
//	{
//		Vector2[] fullArray;
//		List<GunSetupData> gunsData;
//		List<ThrusterSetupData> thrustersData;
//		List<TurretReferenceData> turretsData;
//		GetCurrentData (out fullArray, out gunsData, out thrustersData, out turretsData);
//		if(editType == eSaveLoadType.SpaceShip)
//		{
//			var newShip = new MSpaceshipData ();
//			newShip.verts = fullArray;
//			newShip.guns = gunsData;
//			newShip.thrusters = thrustersData;
//			newShip.turrets = turretsData;
//			newShip.mobility = new SpaceshipData ();
//			int indx = (saveANDloadIndx >= 0) ? saveANDloadIndx : MSpaceShipResources.Instance.spaceships.Count;
//			MSpaceShipResources.Instance.spaceships.Insert(indx, newShip);
//		}
//		else if(editType == eSaveLoadType.Turret)
//		{
//			var newTurret = new TurretSetupData();
//			newTurret.verts = fullArray;
//			newTurret.guns = gunsData;
//			int indx = (saveANDloadIndx >= 0) ? saveANDloadIndx : MSpaceShipResources.Instance.turrets.Count;
//			MSpaceShipResources.Instance.turrets.Insert (indx, newTurret);
//		}
//		else if(editType == eSaveLoadType.Tower)
//		{
//			var newTower = new TowerSetupData();
//			newTower.verts = fullArray;
//			newTower.guns = gunsData;
//			newTower.turrets = turretsData;
//			int indx = (saveANDloadIndx >= 0) ? saveANDloadIndx : MSpaceShipResources.Instance.towers.Count;
//			MSpaceShipResources.Instance.towers.Insert (indx, newTower);
//		}
//		#if UNITY_EDITOR
//		EditorUtility.SetDirty (MSpaceShipResources.Instance);
//		EditorUtility.SetDirty (MGunsResources.Instance);
//		#endif
//	}

//	private object GetObjByType(eSaveLoadType type, int indx)
//	{
//		object obj = null;
//		if(type == eSaveLoadType.SpaceShip)
//			obj = MSpaceShipResources.Instance.spaceships[indx];
//		else if(type == eSaveLoadType.Turret)
//			obj = MSpaceShipResources.Instance.turrets[indx];
//		else if(type == eSaveLoadType.Tower)
//			obj = MSpaceShipResources.Instance.towers[indx];
//		else if(type == eSaveLoadType.BulletGun)
//			obj = MGunsResources.Instance.guns[indx];
//		else if(type == eSaveLoadType.RocketGun)
//			obj = MGunsResources.Instance.rocketLaunchers[indx];
//
//		return obj;
//	}


    [ContextMenu ("Save into prefab")]
    private void SaveOn()
    {
        if (prefab == null) {
        Debug.LogError ("no prefab is set");
        return;
        }

        Vector2[] fullArray;
        List<MGunSetupData> gunsData;
        List<ThrusterSetupData> thrustersData;
        List<MTurretReferenceData> turretsData;
        GetCurrentData (out fullArray, out gunsData, out thrustersData, out turretsData);

        var shape = prefab as IGotShape;
        shape.iverts = fullArray;

        if (prefab is IGotGuns) {
            FillPlaces<MGunSetupData> (gunsData, (prefab as IGotGuns).iguns);
        }

        if (prefab is IGotThrusters) {
            FillPlaces<ThrusterSetupData> (thrustersData, (prefab as IGotThrusters).ithrusters);
        }

        if (prefab is IGotTurrets) {
            FillPlaces<MTurretReferenceData> (turretsData, (prefab as IGotTurrets).iturrets);
        }

//        #if UNITY_EDITOR
//        EditorUtility.SetDirty (MSpaceShipResources.Instance);
//        EditorUtility.SetDirty (MGunsResources.Instance);
//        #endif
    }   


//	[ContextMenu ("Save On")]
//	private void SaveOn()
//	{
//		Vector2[] fullArray;
//		List<GunSetupData> gunsData;
//		List<ThrusterSetupData> thrustersData;
//		List<TurretReferenceData> turretsData;
//		GetCurrentData (out fullArray, out gunsData, out thrustersData, out turretsData);
//		if(saveANDloadIndx >= 0)
//		{
//			object obj = GetObjByType(editType, saveANDloadIndx);
//			
//			(obj as IGotShape).iverts = fullArray;
//			
//			if(obj is IGotGuns)
//				FillPlaces<GunSetupData>(gunsData, (obj as IGotGuns).iguns);
//			
//			if(obj is IGotThrusters)
//				FillPlaces<ThrusterSetupData>(thrustersData, (obj as IGotThrusters).ithrusters);
//			
//			if(obj is IGotTurrets)
//				FillPlaces<TurretReferenceData>(turretsData, (obj as IGotTurrets).iturrets);
//
//			#if UNITY_EDITOR
//			EditorUtility.SetDirty (MSpaceShipResources.Instance);
//			EditorUtility.SetDirty (MGunsResources.Instance);
//			#endif
//		}
//		else
//		{
//			Debug.LogWarning("index is not valid");
//		}
//	}

	private void FillPlaces<T>(List<T> newPlaces, List<T> oldPlaces)
		where T : IGotPlace
	{
		for (int i = 0; i < newPlaces.Count; i++) 
		{
			if(oldPlaces.Count <= i)
			{
				oldPlaces.Add(newPlaces[i]);
			}
			else
			{
				oldPlaces[i].pos = newPlaces[i].pos;
			}
		}
		if(newPlaces.Count < oldPlaces.Count)
		{
			oldPlaces.RemoveRange(newPlaces.Count, oldPlaces.Count - newPlaces.Count);
		}
	}

	void OnDisable()
	{
        Clear ();
	}

    private void Clear()
    {
        foreach(GameObject go in handles)
        {
            DestroyImmediate(go);    
        }
        handles.Clear ();

        foreach(GameObject go in guns)
        {
            DestroyImmediate(go);    
        }
        guns.Clear ();


        foreach(GameObject go in turrets)
        {
            DestroyImmediate(go);    
        }
        turrets.Clear ();

        foreach(GameObject go in thrusters)
        {
            DestroyImmediate(go);    
        }
        thrusters.Clear ();
    }

	private void DuplicateHandler(int dindx)
	{
		if(dindx >= 0 && dindx < handles.Count)
		{
			CreatePosition(Vector3.zero, vright, "handle ", handles);
			for (int i = handles.Count - 1; i >= dindx+1; i--) {
				handles[i].transform.position = handles[i-1].transform.position;
			}
		}
	}

	private GameObject CreatePosition(Vector2 localPos, Vector2 right, string name, List<GameObject> addTo)
	{
		GameObject handle = new GameObject(name + addTo.Count);
		handle.transform.parent = transform;
		handle.transform.localPosition = localPos;
		handle.transform.right = right;
		addTo.Add (handle);
		return handle;
	}

    private bool CheckIsSymmetric( Vector3[] verts )
    {
        if (verts.Length % 2 == 1)
            return false;

        var len = verts.Length;
        for ( int i = 0, cnt = len / 2; i < cnt; i++)
        {
            var v = verts[len - i - 1];
            v.y = -v.y;
            if (Math2d.ApproximatelySame(v, verts[i]) == false)
                return false;
        }

        return true;
    }


    private void Scale(Vector2 vscale)
	{
		foreach (var h in handles) 
		{
			h.transform.localPosition = new Vector3(h.transform.localPosition.x * vscale.x, h.transform.localPosition.y * vscale.y, 0);
		}
	}

	void Update()
	{
		if(handles == null || handles.Count < 2)
			return;

		if(duplicate)
		{
			duplicate = false;
			DuplicateHandler(duplicateIndx);
		}

		if(doScale)
		{
			doScale = false;
			Scale(scaleBy);
		}

		if(scaleUp)
		{
			scaleUp = false;
			Scale(new Vector2(1.08f, 1.08f));
		}

		if(scaleDown)
		{
			scaleDown = false;
			Scale(new Vector2(0.92f, 0.92f));
		}

        if(symmetric != symmetricState)
        {
            symmetricState = symmetric;

            if (symmetric == false)
            {
                for (int i = handles.Count - 1; i >= 0; i--)
                {
                    var pos = handles[i].transform.localPosition;
                    pos.y = -pos.y;
                    CreatePosition(pos, vright, "handle ", handles);
                }
            }
            else
            {
                var newHandlesCnt = handles.Count / 2;
                while (handles.Count > newHandlesCnt)
                {
                    var last = handles.Count - 1;
                    DestroyImmediate(handles[last]);
                    handles.RemoveAt(last);
                }
            }
        }


		Vector2[] v2 = new Vector2[handles.Count];    
		handles = handles.Where (h => h != null).ToList ();
		for(int i = 0; i < handles.Count; i++)
		{
			v2[i] = handles[i].transform.localPosition;    
		}

		var full = symmetricState ? PolygonCreator.GetCompleteVertexes (v2, 1) : new List<Vector2>(v2);
		Vector3[] v3 = new Vector3[full.Count];    
		for(int i = 0; i < full.Count; i++)
		{
			v3[i] = full[i];    
		}

		int[] indx = new int[v3.Length + 1]; 
		for(int i = 0; i < full.Count; i++)
		{
			indx[i] = i;    
		}
		indx [full.Count] = 0;

		Color[] c3 = new Color[v3.Length];    
		for(int i = 0; i < c3.Length; i++)
		{
			c3[i] = Color.white;    
		}

		mesh.Clear ();
		mesh.vertices = v3;
		mesh.colors = c3;
		mesh.SetIndices (indx, MeshTopology.LineStrip, 0);
		mesh.RecalculateBounds();
	}
}