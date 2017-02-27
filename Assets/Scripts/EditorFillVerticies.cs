using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;

[ExecuteInEditMode]
public class EditorFillVerticies : MonoBehaviour 
{
	[SerializeField] bool run = false;
	[SerializeField] float scale = 1f;

	void Update()
	{
		if(run)
		{
			run = false;
#if UNITY_EDITOR
//			Fill();
#endif
		}
	}

//	void Fill()
//	{
//		Debug.LogWarning ("Fill");
//		var vertices = RocketLauncher.missileVertices;
//		float area;
//		Math2d.ScaleVertices (vertices, scale);
//		Vector2 pivot = Math2d.GetMassCenter(vertices, out area);
//		Math2d.ShiftVertices(vertices, -pivot);
//	}
}
