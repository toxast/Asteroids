using UnityEngine;
using System.Collections;

public struct Edge
{
	public Vector2 p1;
	public Vector2 p2;
	
	public Edge(Vector2 a, Vector2 b)
	{
		p1 = a;
		p2 = b;
	}

	/// <summary>
	/// k is relative distance from p1 to p2, k c (0,1);
	/// </summary>
	public Vector2 GetPointOnEdge(float k)
	{
		return p1*(1f-k) + p2*k;
	}

	public Vector2 GetMiddle()
	{
		return (p2 + p1)/2f;
	}

	public float getSqrLength()
	{
		return (p2-p1).sqrMagnitude;
	}
}