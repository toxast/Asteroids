using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AnglesMath : MonoBehaviour {

	[SerializeField] float range = 60;
	[SerializeField] float angle = 30;

	[SerializeField] Vector2 reslut;

	void OnValidate(){
		reslut = Math2d.RotateVertexDeg (new Vector2 (range, 0), angle);
		Debug.LogWarning (reslut);
	}

		
}
