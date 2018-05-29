using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class LazerGameObject: MonoBehaviour{
	public MeshFilter meshFilter;
	void OnDestroy(){
		Destroy (meshFilter.sharedMesh);
	}
}
