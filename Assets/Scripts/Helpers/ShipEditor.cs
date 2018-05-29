using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

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

	[SerializeField] HandleTypeData vertexHandleData;
	[SerializeField] HandleTypeData gunHandleData;
	[SerializeField] HandleTypeData thrusterHandleData;
	[SerializeField] HandleTypeData turretHandleData;

	[SerializeField] List<GameObject> vertexs;
	[SerializeField] List<GameObject> guns;
	[SerializeField] List<GameObject> thrusters;
	[SerializeField] List<GameObject> turrets;

	[SerializeField] bool putPreviousOnDuplicate = false;
	[SerializeField] bool reverse = false;
    [SerializeField] bool symmetric = true;

    [SerializeField] bool scaleUp = false;
	[SerializeField] bool scaleDown = false;
	[SerializeField] bool doScale = false;
	[SerializeField] Vector2 scaleBy = Vector2.one;

	[SerializeField] bool doRotate = false;
	[SerializeField] float rotateby = 45;

	[Header ("shiftVerts")]
	[SerializeField] bool shiftVerts = false;
	[SerializeField] int shiftCount = 1;

	[Header("perfect shape")]
	[SerializeField] bool createPerfectShape = false;
	[SerializeField] float perfectShapeRadius = 1;
	[SerializeField] int perfectShapeSides = 6;

	Vector2 vright = new Vector2(1,0);

    bool symmetricState = true;
    [SerializeField] MonoBehaviour prefab;
	[SerializeField] bool saveToPrefab = false;

	[System.NonSerialized] List<HandleTypeData> handlesData = new List<HandleTypeData>();
	[System.NonSerialized] Dictionary<HandleTypeData, List<GameObject>> handleData2handles = new Dictionary<HandleTypeData, List<GameObject>>();

	void OnDisable() {
		Clear ();
	}

	void OnEnable()
	{
        Clear ();

        if (Application.isPlaying) {
            Debug.LogWarning ("editor should be closed at runtime");
            gameObject.SetActive (false);
            return;
        }

		handlesData.Add (vertexHandleData);
		handlesData.Add (gunHandleData);
		handlesData.Add (thrusterHandleData);
		handlesData.Add (turretHandleData);

		handleData2handles[vertexHandleData] = vertexs;
		handleData2handles[gunHandleData] = guns;
		handleData2handles[thrusterHandleData] = thrusters;
		handleData2handles[turretHandleData] = 	turrets;

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
                    CreatePosition (dataThrusters [i].pos, dataThrusters [i].dir, thrusterHandleData);
                }
            }

            var oIGotGuns = prefab as IGotGuns;
            if (oIGotGuns != null) {
                var dataGuns = oIGotGuns.iguns.ConvertAll (t => t.place);
                for (int i = 0; i < dataGuns.Count; i++) {
					CreatePosition (dataGuns [i].pos, dataGuns [i].dir, gunHandleData);
                }
            }

            var oIGotTurrets = prefab as IGotTurrets;
            if (oIGotTurrets != null) {
                var dataGuns = oIGotTurrets.iturrets.ConvertAll (t => t.place);
                for (int i = 0; i < dataGuns.Count; i++) {
                    CreatePosition (dataGuns [i].pos, dataGuns [i].dir, turretHandleData);
                }
            }
		}

		foreach(var vert in verts)
		{
			if(symmetric == false || vert.y <= 0)
			{
				CreatePosition(vert,  vright, vertexHandleData);
			}
		}
	}

	[ContextMenu ("Create Gun Position")]
	private void CreateGunPosition() {
		CreatePosition (Vector3.zero, vright, gunHandleData);
	}

	[ContextMenu ("Create PerfectVerts")]
	private void CreatePerfectVerts() {
		foreach (GameObject go in vertexs) {
			DestroyImmediate (go);    
		}
		vertexs.Clear ();
		symmetric = false;
		symmetricState = false;
		mesh = GetComponent<MeshFilter> ().sharedMesh;
		var verts = mesh.vertices;
		verts = PolygonCreator.CreatePerfectPolygonVertices (7, sidesNum: 8).ToList ().ConvertAll (v => (Vector3)v).ToArray ();
		foreach (var vert in verts) {
			CreatePosition (vert, vright, vertexHandleData);
		}
	}

	[Header ("fang menu")]
	[SerializeField] int fangs = 3;
	[SerializeField] int fangIndxFrom = 0; //fang will be created from existing handles with this indexes
	[SerializeField] int fangIndxTo = 3;
	[SerializeField] bool createFangShape = false;

	//[ContextMenu ("Create fang shape")]
	private void CreateFangedShape() {
		var fangVerts = vertexs.GetRange (fangIndxFrom, fangIndxTo - fangIndxFrom + 1).ConvertAll(f => (Vector2)f.transform.localPosition);
		foreach(GameObject go in vertexs) {
			DestroyImmediate(go);    
		}
		vertexs.Clear ();
		symmetric = false;
		symmetricState = false;
		float fangBase = (fangVerts.Last () - fangVerts.First ()).magnitude;
		float angleBase = (360f / fangs) * Mathf.Deg2Rad;
		float r = fangBase / Mathf.Sqrt (2f - 2f * Mathf.Cos (angleBase));
		var fangRotatedAngle = Math2d.AngleRad (fangVerts.First () - fangVerts.Last (), new Vector2 (0, 1));
		var fangVertsNormArray = Math2d.RotateVerticesRad (fangVerts.ToArray(), fangRotatedAngle);
		Vector2[] vertices = new Vector2[fangs * (fangVerts.Count-1)];
		float curAngle = 0;
		for (int i = 0; i < fangs; i++) {
			int indx = i * (fangVerts.Count - 1);
			var baseV = r * new Vector2(Mathf.Cos(curAngle), Mathf.Sin(curAngle));
			vertices [indx] = baseV;
			for (int k = 0; k < fangVertsNormArray.Length - 2; k++) {
				var delta = fangVertsNormArray [k + 1] - fangVertsNormArray [k];
				//Debug.Log (delta);
				delta = Math2d.RotateVertex (delta, curAngle - angleBase/2f);
				vertices [indx + 1 + k] = vertices [indx + k] + delta;
			}
			curAngle -= angleBase;
		}
		foreach(var vert in vertices) {
			CreatePosition(vert,  vright, vertexHandleData);
		}
	}
	
	[ContextMenu ("Create Thruster Position")]
	private void CreateThrusterPosition() {
		CreatePosition (Vector3.zero, vright, thrusterHandleData);
	}

	[ContextMenu ("Create Turret Position")]
	private void CreateTurretPosition()
	{
		CreatePosition (Vector3.zero, vright, turretHandleData);
	}

	[ContextMenu ("Reset")]
	void Reset () {
		vertexs.ForEach (h => { h.transform.localPosition = Vector3.zero;});
	}

	private void GetCurrentData(out Vector2[] fullArray, 
	                            out List<MGunSetupData> gunsData,
	                            out List<ParticleSystemsData> thrustersData,
	                            out List<MTurretReferenceData> turretsData) {
		fullArray = new Vector2[1];
		gunsData = new List<MGunSetupData> ();
		thrustersData = new List<ParticleSystemsData> ();
		turretsData = new List<MTurretReferenceData> ();

		Vector2[] v2 = new Vector2[vertexs.Count];    
		for (int i = 0; i < vertexs.Count; i++) {
			v2 [i] = vertexs [i].transform.localPosition;    
		}
		
		var full = symmetricState ? PolygonCreator.GetCompleteVertexes (v2, 1) : new List<Vector2> (v2);
		if (reverse) {
			full.Reverse ();
		}
		
		fullArray = full.ToArray ();
		Vector2 pivot = Vector2.zero;
		if (fullArray.Length > 2) {
			pivot = Math2d.GetMassCenter (fullArray);
		}

		Math2d.ShiftVertices (fullArray, -pivot);

		for (int i = 0; i < fullArray.Length; i++) {
			if (Mathf.Abs (fullArray [i].y) < 0.01f) {
				fullArray [i].y = 0;
			}
		}

		gunsData = new List<MGunSetupData> ();
		foreach (var g in guns) {
			MGunSetupData gd = new MGunSetupData ();
			gd.place = new Place ((Vector2)g.transform.localPosition - pivot, g.transform.right);
			gunsData.Add (gd);
		}
		
		thrustersData = new List<ParticleSystemsData> ();
		foreach (var t in thrusters) {
			ParticleSystemsData td = new ParticleSystemsData ();
			td.place = new Place ((Vector2)t.transform.localPosition - pivot, t.transform.right);
			thrustersData.Add (td);
		}
		
		turretsData = new List<MTurretReferenceData> ();
		foreach (var t in turrets) {
			MTurretReferenceData td = new MTurretReferenceData ();
			td.place = new Place ((Vector2)t.transform.localPosition - pivot, t.transform.right);
			turretsData.Add (td);
		}
	}

    //[ContextMenu ("Save into prefab")]
    private void SaveOn()
    {
		#if UNITY_EDITOR
        if (prefab == null) {
        Debug.LogError ("no prefab is set");
        return;
        }

        Vector2[] fullArray;
        List<MGunSetupData> gunsData;
        List<ParticleSystemsData> thrustersData;
        List<MTurretReferenceData> turretsData;
        GetCurrentData (out fullArray, out gunsData, out thrustersData, out turretsData);

        var shape = prefab as IGotShape;
		if (shape != null) {
			shape.iverts = fullArray;
		}

        if (prefab is IGotGuns) {
            FillPlaces<MGunSetupData> (gunsData, (prefab as IGotGuns).iguns);
        }

        if (prefab is IGotThrusters) {
            FillPlaces<ParticleSystemsData> (thrustersData, (prefab as IGotThrusters).ithrusters);
        }

        if (prefab is IGotTurrets) {
            FillPlaces<MTurretReferenceData> (turretsData, (prefab as IGotTurrets).iturrets);
        }

		EditorUtility.SetDirty (prefab.gameObject);
        #endif
    }   
		

	private void FillPlaces<T>(List<T> newPlaces, List<T> oldPlaces)
		where T : IGotPlace
	{
		for (int i = 0; i < newPlaces.Count; i++) {
			if (oldPlaces.Count <= i) {
				oldPlaces.Add (newPlaces [i]);
			} else {
				oldPlaces [i].pos = newPlaces [i].pos;
			}
		}
		if (newPlaces.Count < oldPlaces.Count) {
			oldPlaces.RemoveRange (newPlaces.Count, oldPlaces.Count - newPlaces.Count);
		}
	}

    private void Clear()
	{
		foreach (var item in handleData2handles) {
			var list = item.Value;
			foreach (var handle in list) {
				DestroyImmediate (handle);    
			}
			list.Clear ();
		}
		handlesData.Clear ();
		handleData2handles.Clear ();
	}

	private GameObject CreatePosition(Vector2 localPos, Vector2 right, HandleTypeData data)
	{
		var list = handleData2handles [data];
		GameObject handle = new GameObject(data.GetHandleName(list.Count));
		var handleScript = handle.AddComponent<EditorHandle> ();
		handleScript.SetData (data);
		handle.transform.parent = transform;
		handle.transform.localPosition = localPos;
		handle.transform.right = right;
		list.Add (handle);
		return handle;
	}

    private bool CheckIsSymmetric( Vector3[] verts )
    {
        var len = verts.Length;
        for ( int idx = 0, cnt = (len + 1) / 2, idxReverse = len - 1; idx < cnt; idx++)
        {
            if (Mathf.Abs(verts[idx].y) < 0.001)
                continue;

            var v = verts[idxReverse--];
            if ( Mathf.Abs(v.x - verts[idx].x) > 0.001 || Mathf.Abs(v.y + verts[idx].y) > 0.001)
                return false;
        }
        return true;
    }

	private void Rotate(float angle){
		foreach (var item in handleData2handles) {
			foreach (var handle in item.Value) {
				handle.transform.localPosition = Math2d.RotateVertexDeg (handle.transform.localPosition, angle);
			}
		}
	}

    private void Scale(Vector2 vscale) {
		foreach (var item in handleData2handles) {
			foreach (var handle in item.Value) {
				handle.transform.localPosition = new Vector3 (handle.transform.localPosition.x * vscale.x, handle.transform.localPosition.y * vscale.y, 0);
			}
		}
	}

	int lastChildrenCount = 0;
	void CheckIfHasADuplicateNewHandle(){
		if (vertexs != null && transform.childCount != lastChildrenCount) {
			lastChildrenCount = transform.childCount;
			foreach(Transform child in transform){
				EditorHandle handle = child.GetComponent<EditorHandle> ();
				Match match = HandleTypeData.GetRegexForNaming.Match(child.name);
				if (match.Success && handle != null) {
					Debug.Log ("found duplicate handle: " + child.name);
					var naming = match.Groups[1].Value;
					var handleData = handlesData.Find (h => h.naming == naming);
					if (handleData != null) {
						handle.SetData (handleData);
						var index = int.Parse (match.Groups [2].Value);
						var handlesList = handleData2handles [handleData];
						if (putPreviousOnDuplicate) {
							handlesList.Insert (index, child.gameObject);
						} else {
							handlesList.Insert (index + 1, child.gameObject);
						}
						RenameList (handleData);
					}

				}
			}

		} 
	}

	void RenameList(HandleTypeData handleData){
		var handlesList = handleData2handles [handleData];
		for (int i = 0; i < handlesList.Count; i++) {
			handlesList [i].name = handleData.GetHandleName (i);
		}
	}

	void CheckForDeletedHandles(){
		foreach (var data in handlesData) {
			var list = handleData2handles [data];
			bool removedAny = false;
			for (int i = list.Count - 1; i >= 0; i--) {
				if (list [i] == null) {
					removedAny = true;
					list.RemoveAt (i);
				}
			}

			if (removedAny) {
				RenameList (data);
			}
		}
	}


	void Update() {
		if (vertexs == null || vertexs.Count < 2)
			return;

		CheckForDeletedHandles ();

		CheckIfHasADuplicateNewHandle ();

		if (doScale) {
			doScale = false;
			Scale (scaleBy);
		}

		if (doRotate) {
			doRotate = false;
			Rotate (rotateby);
		}

		if (scaleUp) {
			scaleUp = false;
			Scale (new Vector2 (1.08f, 1.08f));
		}

		if (scaleDown) {
			scaleDown = false;
			Scale (new Vector2 (0.92f, 0.92f));
		}

		if (createPerfectShape) {
			createPerfectShape = false;
			foreach (GameObject go in vertexs) {
				DestroyImmediate (go);    
			}
			vertexs.Clear ();
			symmetric = false;
			symmetricState = false;
			Vector2[] vertices = PolygonCreator.CreatePerfectPolygonVertices (perfectShapeRadius, perfectShapeSides);
			foreach (var vert in vertices) {
				CreatePosition (vert, vright, vertexHandleData);
			}
		}

		if (shiftVerts) {
			if (symmetric || symmetricState) {
				shiftVerts = false;
				symmetric = false;
				symmetricState = false;
			} else {
				shiftVerts = false;
				for (int i = 0; i < shiftCount; i++) {
					vertexs.Insert (0, vertexs.Last ());
					vertexs.RemoveAt (vertexs.Count - 1);
				}
				RenameList (vertexHandleData);
			}
		}

		if (symmetric != symmetricState) {
			symmetricState = symmetric;
			if (symmetric == false) {
				for (int i = vertexs.Count - 1; i >= 0; i--) {
					var pos = vertexs [i].transform.localPosition;
					pos.y = -pos.y;
					CreatePosition (pos, vright, vertexHandleData);
				}
			} else {
				var newHandlesCnt = vertexs.Count / 2;
				while (vertexs.Count > newHandlesCnt) {
					var last = vertexs.Count - 1;
					DestroyImmediate (vertexs [last]);
					vertexs.RemoveAt (last);
				}
			}
		}

		if (createFangShape) {
			createFangShape = false;
			CreateFangedShape ();
		}

		Vector2[] v2 = new Vector2[vertexs.Count];    
		for (int i = 0; i < vertexs.Count; i++) {
			v2 [i] = vertexs [i].transform.localPosition;    
		}

		var full = symmetricState ? PolygonCreator.GetCompleteVertexes (v2, 1) : new List<Vector2> (v2);
		Vector3[] v3 = new Vector3[full.Count];    
		for (int i = 0; i < full.Count; i++) {
			v3 [i] = full [i];    
		}

		int[] indx = new int[v3.Length + 1]; 
		for (int i = 0; i < full.Count; i++) {
			indx [i] = i;    
		}
		indx [full.Count] = 0;

		Color[] c3 = new Color[v3.Length];    
		for (int i = 0; i < c3.Length; i++) {
			c3 [i] = Color.white;    
		}

		mesh.Clear ();
		mesh.vertices = v3;
		mesh.colors = c3;
		mesh.SetIndices (indx, MeshTopology.LineStrip, 0);
		mesh.RecalculateBounds ();

		if (saveToPrefab) {
			saveToPrefab = false;
			SaveOn ();
		}
	}
}