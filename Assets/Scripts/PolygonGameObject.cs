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

	public void SetColor(Color col)
	{
		int len = mesh.colors.Length;
		Color [] colors = new Color[len];
		for (int i = 0; i < len; i++) 
		{
			colors[i] = col;
		}
		mesh.colors = colors;
	}

}
