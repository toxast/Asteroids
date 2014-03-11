using UnityEngine;
using System.Collections;

public class PolygonGameObject : MonoBehaviour 
{
	public Transform cacheTransform;
	public Polygon polygon;
	public Mesh mesh;

	void Awake () 
	{
		cacheTransform = transform;
	}

	public void SetPolygon(Polygon polygon)
	{
		this.polygon = polygon;
	}

}
