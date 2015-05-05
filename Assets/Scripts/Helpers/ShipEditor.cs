using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]

public class ShipEditor : MonoBehaviour 
{
	public enum eSaveLoadType
	{
		SpaceShip,
		Turret,
		Tower,
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

	[SerializeField] bool doScale = false;
	[SerializeField] Vector2 scaleBy = Vector2.one;
	[SerializeField] int saveANDloadIndx = -1;
	[SerializeField] eSaveLoadType editType;

	Vector2 vright = new Vector2(1,0);

	void OnEnable()
	{
		duplicateIndx = 0;
		duplicate = false;
		
		mesh = GetComponent<MeshFilter>().sharedMesh;
		var verts = mesh.vertices;
		if(saveANDloadIndx >= 0)
		{
			object obj = null;
			if(editType == eSaveLoadType.SpaceShip)
				obj = SpaceshipsResources.Instance.spaceships[saveANDloadIndx];
			else if(editType == eSaveLoadType.Turret)
				obj = SpaceshipsResources.Instance.turrets[saveANDloadIndx];
			else if(editType == eSaveLoadType.Tower)
				obj = SpaceshipsResources.Instance.towers[saveANDloadIndx];

			verts = (obj as IGotShape).iverts.ToList().ConvertAll( v => (Vector3)v).ToArray();

			var oIGotThrusters = obj as IGotThrusters;
			if(oIGotThrusters != null)
			{
				var dataThrusters = oIGotThrusters.ithrusters.ConvertAll( t => t.place);
				for (int i = 0; i < dataThrusters.Count; i++) 
				{
					CreatePosition(dataThrusters[i].pos, dataThrusters[i].dir, "truster ", thrusters);
				}
			}

			var oIGotGuns = obj as IGotGuns;
			if(oIGotGuns != null)
			{
				var dataGuns = oIGotGuns.iguns.ConvertAll( t => t.place);
				for (int i = 0; i < dataGuns.Count; i++) 
				{
					CreatePosition(dataGuns[i].pos, dataGuns[i].dir, "gun ", guns);
				}
			}

			var oIGotTurrets = obj as IGotTurrets;
			if(oIGotTurrets != null)
			{
				var dataGuns = oIGotTurrets.iturrets.ConvertAll( t => t.place);
				for (int i = 0; i < dataGuns.Count; i++) 
				{
					CreatePosition(dataGuns[i].pos, dataGuns[i].dir, "turret ", turrets);
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


	[ContextMenu ("Custom action")]
	private void CustomAction()
	{
		Vector2[] v2 = new Vector2[handles.Count];    
		for(int i = 0; i < handles.Count; i++)
		{
			v2[i] = handles[i].transform.localPosition;    
		}
		
		var verts = PolygonCreator.GetCompleteVertexes (v2, 1).ToArray();
		var pivot = Math2d.GetMassCenter (verts);
		Math2d.ShiftVertices(verts, -pivot);
		GunsResources.Instance.rocketLaunchers [5].baseData.vertices = verts;
		
		//		string s = string.Empty;
		//		foreach (var v in verts) 
		//		{
		//			s += string.Format("new Vector2 ({0:0.00}f, {1:0.00}f),", v.x, v.y);
		//			s += '\n';
		//		}
		//		Debug.LogWarning (s);
	}

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
	                            out List<GunSetupData> gunsData,
	                            out List<ThrusterSetupData> thrustersData,
	                            out List<TurretReferenceData> turretsData)
	{
		Vector2[] v2 = new Vector2[handles.Count];    
		for(int i = 0; i < handles.Count; i++)
		{
			v2[i] = handles[i].transform.localPosition;    
		}
		
		var full = PolygonCreator.GetCompleteVertexes (v2, 1);
		if(reverse)
		{
			full.Reverse();
		}
		
		fullArray = full.ToArray ();
		var pivot = Math2d.GetMassCenter (fullArray);
		Math2d.ShiftVertices(fullArray, -pivot);
		
		gunsData = new List<GunSetupData> ();
		foreach(var g in guns)
		{
			GunSetupData gd = new GunSetupData();
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
		
		turretsData = new List<TurretReferenceData> ();
		foreach(var t in turrets)
		{
			TurretReferenceData td = new TurretReferenceData();
			td.place = new Place((Vector2)t.transform.localPosition - pivot, t.transform.right);
			turretsData.Add(td);
		}
	}

	[ContextMenu ("Insert")]
	private void Insert()
	{
		Vector2[] fullArray;
		List<GunSetupData> gunsData;
		List<ThrusterSetupData> thrustersData;
		List<TurretReferenceData> turretsData;
		GetCurrentData (out fullArray, out gunsData, out thrustersData, out turretsData);
		if(editType == eSaveLoadType.SpaceShip)
		{
			var newShip = new FullSpaceShipSetupData ();
			newShip.verts = fullArray;
			newShip.guns = gunsData;
			newShip.thrusters = thrustersData;
			newShip.turrets = turretsData;
			newShip.physicalParameters = new SpaceshipData ();
			int indx = (saveANDloadIndx >= 0) ? saveANDloadIndx : SpaceshipsResources.Instance.spaceships.Count;
			SpaceshipsResources.Instance.spaceships.Insert(indx, newShip);
		}
		else if(editType == eSaveLoadType.Turret)
		{
			var newTurret = new TurretSetupData();
			newTurret.verts = fullArray;
			newTurret.guns = gunsData;
			int indx = (saveANDloadIndx >= 0) ? saveANDloadIndx : SpaceshipsResources.Instance.turrets.Count;
			SpaceshipsResources.Instance.turrets.Add (newTurret);
		}
		else if(editType == eSaveLoadType.Tower)
		{
			var newTower = new TowerSetupData();
			newTower.verts = fullArray;
			newTower.guns = gunsData;
			newTower.turrets = turretsData;
			int indx = (saveANDloadIndx >= 0) ? saveANDloadIndx : SpaceshipsResources.Instance.towers.Count;
			SpaceshipsResources.Instance.towers.Add (newTower);
		}
	}

	[ContextMenu ("Save On")]
	private void SaveOn()
	{
		Vector2[] fullArray;
		List<GunSetupData> gunsData;
		List<ThrusterSetupData> thrustersData;
		List<TurretReferenceData> turretsData;
		GetCurrentData (out fullArray, out gunsData, out thrustersData, out turretsData);
		if(saveANDloadIndx >= 0)
		{
			object obj = null;
			if(editType == eSaveLoadType.SpaceShip)
				obj = SpaceshipsResources.Instance.spaceships[saveANDloadIndx];
			else if(editType == eSaveLoadType.Turret)
				obj = SpaceshipsResources.Instance.turrets[saveANDloadIndx];
			else if(editType == eSaveLoadType.Tower)
				obj = SpaceshipsResources.Instance.towers[saveANDloadIndx];
			
			(obj as IGotShape).iverts = fullArray;
			
			if(obj is IGotGuns)
				FillPlaces<GunSetupData>(gunsData, (obj as IGotGuns).iguns);
			
			if(obj is IGotThrusters)
				FillPlaces<ThrusterSetupData>(thrustersData, (obj as IGotThrusters).ithrusters);
			
			if(obj is IGotTurrets)
				FillPlaces<TurretReferenceData>(turretsData, (obj as IGotTurrets).iturrets);
		}
		else
		{
			Debug.LogWarning("index is not valid");
		}
	}

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
			foreach (var h in handles) 
			{
				h.transform.localPosition = new Vector3(h.transform.localPosition.x * scaleBy.x, h.transform.localPosition.y * scaleBy.y, 0);
			}
		}


		Vector2[] v2 = new Vector2[handles.Count];    
		handles = handles.Where (h => h != null).ToList ();
		for(int i = 0; i < handles.Count; i++)
		{
			v2[i] = handles[i].transform.localPosition;    
		}

		var full = PolygonCreator.GetCompleteVertexes (v2, 1);
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

		mesh.vertices = v3;
		mesh.SetIndices (indx, MeshTopology.LineStrip, 0);
		mesh.RecalculateBounds();
	}
}