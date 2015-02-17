using UnityEngine;
using UnityEditor;
using System.Collections;

[ExecuteInEditMode]
public class EditorFillVerticies : MonoBehaviour 
{
	[SerializeField] bool run = false;

	void Update()
	{
		if(run)
		{
			run = false;
			Fill();
		}
	}

	void Fill()
	{
		Debug.LogWarning ("Fill");

		var verticies = PolygonCreator.GetRectShape(0.4f, 0.2f);

		Polygon p = new Polygon (verticies);

		GunsResources.Instance.guns [0].vertices = p.vertices;
		EditorUtility.SetDirty (GunsResources.Instance);
	}
}
