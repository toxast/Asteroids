using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]

public class VertHandler : MonoBehaviour 
{
	Mesh mesh;
	Vector3 vertPos;
	[SerializeField] bool doPrint = false;
	[SerializeField] List<GameObject> handles;

	[SerializeField] int duplicateIndx = 0;
	[SerializeField] bool duplicate = false;

	[ContextMenu ("Reset")]
	void Reset () {
		handles.ForEach (h =>
		                 { h.transform.localPosition = Vector3.zero;}
				);
	}

	void OnEnable()
	{
		duplicateIndx = 0;
		duplicate = false;



		mesh = GetComponent<MeshFilter>().sharedMesh;
		var verts = mesh.vertices;
		foreach(Vector3 vert in verts)
		{
			if(vert.y <= 0)
			{
				vertPos = transform.TransformPoint(vert);
				GameObject handle = new GameObject("handle " + handles.Count);
				handle.transform.position = vertPos;
				handle.transform.parent = transform;
				handle.tag = "handle";
				handles.Add(handle);
			}
		}
	}
	
	void OnDisable()
	{
		foreach(GameObject handle in handles)
		{
			DestroyImmediate(handle);    
		}
		handles.Clear ();
	}


	private void DuplicateHandler(int dindx)
	{
		if(dindx >= 0 && dindx < handles.Count)
		{
			GameObject handle = new GameObject("handle " + dindx);
			handle.transform.position = handles[dindx].transform.position;
			handle.transform.parent = transform;
			handle.tag = "handle";
			handles.Insert(dindx, handle);

			for (int i = dindx; i < handles.Count; i++) {
				handles[i].name = "handle " + i;
			}
		}
	}

	private void PrintIt(Vector2[] verts)
	{
		string s = string.Empty;
		foreach (var v in verts) 
		{
			s += string.Format("new Vector2 ({0:0.00}f, {1:0.00}f),", v.x, v.y);
			s += '\n';
		}
		Debug.LogWarning (s);

	}

	void Update()
	{
		if(duplicate)
		{
			duplicate = false;
			DuplicateHandler(duplicateIndx);
		}

		if(handles == null || handles.Count < 2)
			return;

		Vector2[] v2 = new Vector2[handles.Count];    
		handles = handles.Where (h => h != null).ToList ();
		for(int i = 0; i < handles.Count; i++)
		{
			v2[i] = handles[i].transform.localPosition;    
		}

		var full = PolygonCreator.GetCompleteVertexes (v2, 1);

		if(doPrint)
		{
			doPrint = false;
			PrintIt(full.ToArray());
		}

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