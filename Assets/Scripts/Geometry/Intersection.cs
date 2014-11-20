using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Intersection {

	public bool haveIntersection {get; private set;}
	public Vector2 intersection {get; private set;}

	/// <summary>
	/// Determines intersection of two segments (a1 -> a2) and (b1-> b2)
	/// If there is an intersection haveIntersection will have the value of true, false - otherwise
	/// intersection will be stored in intersection;
	/// </summary>
	public Intersection(
		Vector2 a1, Vector2 a2, 
		Vector2 b1, Vector2 b2)
	{
		float d = (a1.x -a2.x)*(b1.y-b2.y) - (a1.y-a2.y)*(b1.x-b2.x);

	    if (d == 0f) 
		{
			//Debug.Log(a1 + "-" + a2 + " " + b1 + "-" + b2 + ": return");
			return;
		}

		float P1 = a1.x*a2.y - a1.y*a2.x;
		float P2 = b1.x*b2.y - b1.y*b2.x;
		float xi = ( (b1.x - b2.x)*P1 - (a1.x - a2.x)*P2 ) / d;
		float yi = ( (b1.y - b2.y)*P1 - (a1.y - a2.y)*P2 ) / d;
	    
		float delta = 0.0001f;

		intersection = new Vector2(xi, yi);
		haveIntersection =  
			(xi >= Mathf.Min(a1.x, a2.x)-delta && xi <= Mathf.Max(a1.x, a2.x)+delta) &&
			(xi >= Mathf.Min(b1.x, b2.x)-delta && xi <= Mathf.Max(b1.x, b2.x)+delta) &&
			(yi >= Mathf.Min(a1.y, a2.y)-delta && yi <= Mathf.Max(a1.y, a2.y)+delta) &&
			(yi >= Mathf.Min(b1.y, b2.y)-delta && yi <= Mathf.Max(b1.y, b2.y)+delta);

		//Debug.Log(VectorToStr(a1)  + "-" + VectorToStr(a2) + " " + VectorToStr(b1) + "-" + VectorToStr(b2) + ": " + haveIntersection + " " + VectorToStr(intersection));
	}

	static private string VectorToStr(Vector2 a)
	{
		return "[" + a.x + ", " + a.y + "]"; 
	}

	/// <summary>
	/// Returns the intersections between segment <e> and the <edges>.
	/// If edges have common points - this edges should be consuquental in the array
	/// Otherwise the result may contain duplicate intersection instances.
	/// </summary>
	public static List<Vector2> GetDifferentIntersections(Edge e, Edge[] edges)
	{
		List<Vector2> intersections = new List<Vector2> ();
		foreach(Edge edge in edges)
		{
			Intersection insc = new Intersection(e.p1, e.p2, edge.p1, edge.p2); 
			bool sameAsPrevious = intersections.Any() && Math2d.ApproximatelySame(intersections.Last(), insc.intersection);
			if(insc.haveIntersection && !sameAsPrevious)
			{
				intersections.Add(insc.intersection);
			}
		}
		return intersections;
	}


	public static List<Intersection> GetIntersections(Edge e, Edge[] edges)
	{
		List<Intersection> intersections = new List<Intersection> ();
		foreach(Edge edge in edges)
		{
			intersections.Add(new Intersection(e.p1, e.p2, edge.p1, edge.p2));
		}
		return intersections;
	}


	static public void Test()
	{
		//parralel
		Intersection i = new Intersection(new Vector2(0,1), new Vector2(0,2), new Vector2(1,1), new Vector2(1,2));
		Debug.LogWarning(i.haveIntersection + " " + i.intersection);

		//intersects
		Intersection i2 = new Intersection(new Vector2(0,1), new Vector2(3,2), new Vector2(2,1), new Vector2(1,2));
		Debug.LogWarning(i2.haveIntersection + " " + i2.intersection);

		//fringe dot intersection
		Intersection i3 = new Intersection(new Vector2(0,1), new Vector2(3.123f, 2.123f), new Vector2(2,1), new Vector2(3.123f, 2.123f));
		Debug.LogWarning(i3.haveIntersection + " " + i3.intersection);

		//lines intersect, segments - not
		Intersection i4 = new Intersection(new Vector2(0,1), new Vector2(3,2), new Vector2(2,1), new Vector2(3,0));
		Debug.LogWarning(i4.haveIntersection + " " + i4.intersection);

		//one segment is vertical
		Intersection i5 = new Intersection(new Vector2(1,1), new Vector2(1,4), new Vector2(4,2), new Vector2(0,9));
		Debug.LogWarning(i5.haveIntersection + " " + i5.intersection);

		//one horisontal, other - vertical
		Intersection i6 = new Intersection(new Vector2(-2.2f, -2.0f), new Vector2(-2.2f, 2.0f), new Vector2(1.2f, 0.0f), new Vector2(-11.6f, 0.0f));
		Debug.LogWarning(i6.haveIntersection + " " + i6.intersection);

	}
}
