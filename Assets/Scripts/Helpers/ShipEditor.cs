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

	[SerializeField] bool doRotate = false;
	[SerializeField] float rotateby = 45;

	[Header("perfect shape")]
	[SerializeField] bool createPerfectShape = false;
	[SerializeField] float perfectShapeRadius = 1;
	[SerializeField] int perfectShapeSides = 6;

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
			if(symmetric == false || vert.y <= 0)
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

	[ContextMenu ("Create PerfectVerts")]
	private void CreatePerfectVerts()
	{
		foreach(GameObject go in handles)
		{
			DestroyImmediate(go);    
		}
		handles.Clear ();
		symmetric = false;
		symmetricState = false;
		mesh = GetComponent<MeshFilter>().sharedMesh;
		var verts = mesh.vertices;
		verts = PolygonCreator.CreatePerfectPolygonVertices (7, sidesNum: 8).ToList().ConvertAll(v => (Vector3)v).ToArray();
		foreach(var vert in verts)
		{
			CreatePosition(vert,  vright, "handle ", handles);
		}
	}

	[Header ("fang menu")]
	[SerializeField] int fangs = 3;
	[SerializeField] int fangIndxFrom = 0; //fang will be created from existing handles with this indexes
	[SerializeField] int fangIndxTo = 3;

	[ContextMenu ("Create fang shape")]
	private void CreateFangedShape() {
		var fangVerts = handles.GetRange (fangIndxFrom, fangIndxTo - fangIndxFrom + 1).ConvertAll(f => (Vector2)f.transform.localPosition);
		foreach(GameObject go in handles) {
			DestroyImmediate(go);    
		}
		handles.Clear ();
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
				Debug.Log (delta);
				delta = Math2d.RotateVertex (delta, curAngle - angleBase/2f);
				vertices [indx + 1 + k] = vertices [indx + k] + delta;
			}
			curAngle -= angleBase;
		}
		foreach(var vert in vertices) {
			CreatePosition(vert,  vright, "handle ", handles);
		}
	}
	
	[ContextMenu ("Create Thruster Position")]
	private void CreateThrusterPosition() {
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
	                            out List<ParticleSystemsData> thrustersData,
	                            out List<MTurretReferenceData> turretsData)
	{
		fullArray = new Vector2[1];
        gunsData = new List<MGunSetupData> ();
		thrustersData = new List<ParticleSystemsData> ();
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
		Vector2 pivot = Vector2.zero;
		if (fullArray.Length > 2) {
			pivot = Math2d.GetMassCenter (fullArray);
		}

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
		
		thrustersData = new List<ParticleSystemsData> ();
		foreach(var t in thrusters)
		{
			ParticleSystemsData td = new ParticleSystemsData();
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
		foreach (var h in handles) 
		{
			h.transform.localPosition = Math2d.RotateVertexDeg (h.transform.localPosition, angle);
		}
	}


    private void Scale(Vector2 vscale)
	{
		foreach (var h in handles) 
		{
			h.transform.localPosition = new Vector3(h.transform.localPosition.x * vscale.x, h.transform.localPosition.y * vscale.y, 0);
		}

		foreach (var h in turrets) 
		{
			h.transform.localPosition = new Vector3(h.transform.localPosition.x * vscale.x, h.transform.localPosition.y * vscale.y, 0);
		}

		foreach (var h in guns) 
		{
			h.transform.localPosition = new Vector3(h.transform.localPosition.x * vscale.x, h.transform.localPosition.y * vscale.y, 0);
		}

		foreach (var h in thrusters) 
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

		if(doRotate) {
			doRotate = false;
			Rotate(rotateby);
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

		if (createPerfectShape) {
			createPerfectShape = false;
			foreach(GameObject go in handles) {
				DestroyImmediate(go);    
			}
			handles.Clear ();
			symmetric = false;
			symmetricState = false;
			Vector2[] vertices = PolygonCreator.CreatePerfectPolygonVertices(perfectShapeRadius, perfectShapeSides);
			foreach(var vert in vertices) {
				CreatePosition(vert,  vright, "handle ", handles);
			}
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


	public static string[] GetAllPrefabPaths () {
		string[] temp = AssetDatabase.GetAllAssetPaths();
		List<string> result = new List<string>();
		foreach ( string s in temp ) {
			if ( s.Contains( ".prefab" ) ) result.Add( s );
		}
		return result.ToArray();
	}

	public static List<T> LoadPrefabsContaining<T>() where T : UnityEngine.Component
	{
		List<T> result = new List<T>();
		var allPrefabs = GetAllPrefabPaths ();
		foreach (var path in allPrefabs)
		{
			int resIndx = path.IndexOf ("Resources");
			if (resIndx < 0)
				continue;
			
			string newpath = path.Substring (resIndx + "Resources/".Length );
			newpath = newpath.Substring (0, newpath.Length - ".prefab".Length);

			var cmp = Resources.Load<T>(newpath);
			if (cmp != null)
			{
				Debug.LogWarning (newpath);
				result.Add(cmp);
			}
		}
		return result;
	}

	[ContextMenu ("CustomAction")]
	public void MyCustomAction() {
//
		var list3 = LoadPrefabsContaining<MCometData> ();
		foreach (var item in list3) {
			item.powerupData.effectData.color = item.color;
			EditorUtility.SetDirty (item.gameObject);
		}
//
//		var list4 = LoadPrefabsContaining<MFlamerGunData> ();
//		foreach (var item in list4) {
//			item.deathData.destructionType = PolygonGameObject.DestructionType.eDisappear;
//			item.deathData.createExplosionOnDeath = false;
//			EditorUtility.SetDirty (item.gameObject);
//		}
//
//		var list5 = LoadPrefabsContaining<MFlamerGunData> ();
//		foreach (var item in list5) {
//			item.deathData.destructionType = PolygonGameObject.DestructionType.eDisappear;
//			item.deathData.createExplosionOnDeath = false;
//			EditorUtility.SetDirty (item.gameObject);
//		}
//
//		var list = LoadPrefabsContaining<MRocketGunData> ();
//		foreach (var item in list) {
//			item.deathData.destructionType = PolygonGameObject.DestructionType.eDisappear;
//			item.deathData.createExplosionOnDeath = true;
//			item.deathData.instantExplosion = true;
//			item.deathData.overrideExplosionDamage = item.overrideExplosionDamage;
//			item.deathData.overrideExplosionRange= item.overrideExplosionRadius;
//			EditorUtility.SetDirty (item.gameObject);
//		}
//
//		var list2 = LoadPrefabsContaining<MMinesGunData> ();
//		foreach (var item in list2) {
//			item.deathData.destructionType = PolygonGameObject.DestructionType.eComplete;
//			item.deathData.createExplosionOnDeath = true;
//			item.deathData.instantExplosion = true;
//			item.deathData.overrideExplosionDamage = item.overrideExplosionDamage;
//			item.deathData.overrideExplosionRange= item.overrideExplosionRadius;
//			EditorUtility.SetDirty (item.gameObject);
//		}
//

		AssetDatabase.SaveAssets();
	}
}