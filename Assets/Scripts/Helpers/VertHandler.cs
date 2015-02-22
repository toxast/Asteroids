using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]

public class VertHandler : MonoBehaviour 
{
	Mesh mesh;
	Vector3 vertPos;
	[SerializeField] List<GameObject> handles;
	[SerializeField] List<GameObject> guns;
	[SerializeField] List<GameObject> thrusters;

	[SerializeField] int duplicateIndx = 0;
	[SerializeField] bool duplicate = false;
	[SerializeField] bool reverse = false;

	[SerializeField] bool doScale = false;
	[SerializeField] Vector2 scaleBy = Vector2.one;


	void OnEnable()
	{
		duplicateIndx = 0;
		duplicate = false;
		
		mesh = GetComponent<MeshFilter>().sharedMesh;
		//var verts = mesh.vertices;
		var verts = SpaceshipsData.valaSpaceship;
		//var verts =SpaceshipsResources.Instance.spaceships[ ].verts;
		foreach(Vector3 vert in verts)
		{
			if(vert.y <= 0)
			{
				vertPos = transform.TransformPoint(vert);
				GameObject handle = new GameObject("handle " + handles.Count);
				handle.transform.position = vertPos;
				handle.transform.parent = transform;
				//handle.tag = "handle";
				handles.Add(handle);
			}
		}
	}

	[ContextMenu ("Reset")]
	void Reset () {
		handles.ForEach (h =>
		                 { h.transform.localPosition = Vector3.zero;}
				);
	}

	[ContextMenu ("Save")]
	private void PrintIt()
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
//		if(reverse)
//		{
//			List<Vector2> rv = new List<Vector2>(full);
//			rv.Reverse();
//			Print(rv.ToArray());
//		}
//		else
//		{
//			Print(full.ToArray());
//		}

		var sdata = new FullSpaceShipSetupData ();

		var fullArray = full.ToArray ();
		var pivot = Math2d.GetMassCenter (fullArray);
		Math2d.ShiftVertices(fullArray, -pivot);
		sdata.verts = fullArray;

		List<GunSetupData> gunsData = new List<GunSetupData> ();
		foreach(var g in guns)
		{
			GunSetupData gd = new GunSetupData();
			gd.place = new Place((Vector2)g.transform.localPosition - pivot, g.transform.right);
			gunsData.Add(gd);
		}
		sdata.guns = gunsData;


		List<ThrusterSetupData> thrustersData = new List<ThrusterSetupData> ();
		foreach(var t in thrusters)
		{
			ThrusterSetupData td = new ThrusterSetupData();
			td.place = new Place((Vector2)t.transform.localPosition - pivot, t.transform.right);
			thrustersData.Add(td);
		}
		sdata.thrusters = thrustersData;

		sdata.physicalParameters = new SpaceshipData ();


		SpaceshipsResources.Instance.spaceships.Add (sdata);
		//var sdata = new FullSpaceShipSetupData(
		//SpaceshipsRecources.Instance.spaceships.Add( 

		//ObjectsCreator.CreateSpaceShip<EnemySpaceShip> (sdata);
	}
	
	private void Print(Vector2[] verts)
	{
		string s = string.Empty;
		foreach (var v in verts) 
		{
			s += string.Format("new Vector2 ({0:0.00}f, {1:0.00}f),", v.x, v.y);
			s += '\n';
		}
		Debug.LogWarning (s);
	}




	void OnDisable()
	{
		foreach(GameObject handle in handles)
		{
			DestroyImmediate(handle);    
		}
		handles.Clear ();

		foreach(GameObject gun in guns)
		{
			DestroyImmediate(gun);    
		}
		guns.Clear ();

		foreach(GameObject th in thrusters)
		{
			DestroyImmediate(th);    
		}
		thrusters.Clear ();
	}

	

	private void DuplicateHandler(int dindx)
	{
		if(dindx >= 0 && dindx < handles.Count)
		{
			GameObject handle = new GameObject("handle " + dindx);
			handle.transform.position = handles[dindx].transform.position;
			handle.transform.parent = transform;
			//handle.tag = "handle";
			handles.Insert(dindx, handle);

			for (int i = dindx; i < handles.Count; i++) {
				handles[i].name = "handle " + i;
			}
		}
	}

	[ContextMenu ("Create Gun Position")]
	private void CreateGunPosition()
	{
		GameObject handle = new GameObject("gun" + guns.Count);
		handle.transform.parent = transform;
		handle.transform.localPosition = Vector3.zero;
		guns.Add (handle);
	}

	[ContextMenu ("Create Thruster Position")]
	private void CreateThrusterPosition()
	{
		GameObject handle = new GameObject("thruster" + thrusters.Count);
		handle.transform.parent = transform;
		handle.transform.localPosition = Vector3.zero;
		thrusters.Add (handle);
	}

	void Update()
	{
//		if(Input.GetKeyDown(KeyCode.LeftShift))
//		{
//			var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
//			Debug.LogWarning(ray.origin);
		//}
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