using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorHandle : MonoBehaviour {
	Color color;
	float size;
	bool drawDirection;
	public void SetData(HandleTypeData data){
		this.color = data.gizmoColor;
		this.size = data.gizmoSize;
		this.drawDirection = data.drawDirection;
	}

	void OnDrawGizmos() {
		Gizmos.color = color;
		Gizmos.DrawSphere(transform.position, size);
		if (drawDirection) {
			Gizmos.DrawRay(transform.position, transform.right);
		}
	}
}
