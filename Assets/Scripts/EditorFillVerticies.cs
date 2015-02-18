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

		var vertices = RocketLauncher.missileVertices;

		float area;
		Math2d.ScaleVertices (vertices, 1.3f);
		Vector2 pivot = Math2d.GetMassCenter(vertices, out area);
		Math2d.ShiftVertices(vertices, -pivot);

		GunsResources.Instance.rocketLaunchers [1].baseData.vertices = vertices;
		EditorUtility.SetDirty (GunsResources.Instance);
	}
}
