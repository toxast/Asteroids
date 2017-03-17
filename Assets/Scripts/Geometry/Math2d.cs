using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

public static class Math2d {
    public static float PIdiv180 = Mathf.PI / 180f;

    static Vector2 right = new Vector2(1, 0);

	public static void SetStartColor(this ParticleSystem ps, Color col){
		var pmain = ps.main;
		var stCol = pmain.startColor;
		stCol.color = col;
		pmain.startColor = stCol;
	}

	public static void SetDefaultValues(this List<ParticleSystemsData> effects){
		for (int i = 0; i < effects.Count; i++) {
			if (effects [i].prefab == null) {
				effects [i] = new ParticleSystemsData ();
			}
		}
	}

    private static System.Random rng = new System.Random();
    public static void Shuffle<T>(this IList<T> list) {
        int n = list.Count;
        while (n > 1) {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    static public float DotProduct(ref Vector2 v1, ref Vector2 v2) {
        return v1.x * v2.x + v1.y * v2.y;
    }

    static public float Cos(ref Vector2 v1, ref Vector2 v2) {
        return DotProduct(ref v1, ref v2) / (v1.magnitude * v2.magnitude);
    }

    static public float Cos(Vector2 v1, Vector2 v2) {
        return DotProduct(ref v1, ref v2) / (v1.magnitude * v2.magnitude);
    }


    static public float AngleRad(Vector2 v1, Vector2 v2) {
        return AngleRad(ref v1, ref v2);
    }
    /// <summary>
    /// returns angle [0, 2*Pi] to rotate from v1 to v2 counterclockwise;
    /// </summary>
    static public float AngleRad(ref Vector2 v1, ref Vector2 v2) {
        float sign = Mathf.Sign(Cross(ref v1, ref v2));
        float angle = Mathf.Acos(Cos(ref v1, ref v2));
        if (sign > 0) {
            return angle;
        } else {
            return 2 * Mathf.PI - angle;
        }
    }

    static public Vector2 MakeRight(Vector2 v) {
        return new Vector2(v.y, -v.x);
    }

    static public float ClosestAngleBetweenNormalizedDegAbs(Vector2 vn1, Vector2 vn2) {
        var rad = ClosestAngleBetweenNormalizedRad(vn1, vn2);
        return Mathf.Abs(rad * Mathf.Rad2Deg);
    }

    static public float ClosestAngleBetweenNormalizedRad(Vector2 vn1, Vector2 vn2) {
        var cos = DotProduct(ref vn1, ref vn2);
        var rad = Mathf.Acos(Mathf.Clamp(cos, -1, 1));
        return rad;
    }

    static public bool Chance(float chance) {
        return chance > UnityEngine.Random.Range(0f, 1f);
    }

    static public int Roll(List<float> weights) {
        float sum = 0;
        foreach (var item in weights) {
            sum += item;
        }
        if (sum == 0) {
            return -1;
        }
        float rnd = Random.Range(0f, 1f);
        float sum2 = 0;
        for (int i = 0; i < weights.Count; i++) {
            sum2 += weights[i] / sum;
            if (rnd <= sum2) {
                return i;
            }
        }
        return weights.Count - 1;
    }

    static public float DeltaAngleDeg(float fromAngle, float toAngle) {
        float diff = toAngle - fromAngle;

        if (diff > 180) {
            diff = diff - 360;
        } else if (diff < -180) {
            diff = 360 + diff;
        }

        return diff;
    }

    /// <summary>
    /// returns angle [0, 2*Pi] to rotate from vector (1, 0) to v counterclockwise;
    /// </summary>
    static public float GetRotationRad(ref Vector2 v) {
        return AngleRad(ref right, ref v);
    }

    static public float GetRotationRad(Vector2 v) {
        return AngleRad(ref right, ref v);
    }

    static public float GetRotationDg(Vector2 v) {
        return GetRotationRad(ref v) * Mathf.Rad2Deg;
    }

    static public Vector2 GetMassCenter(Vector2[] vertices) {
        float area;
        return GetMassCenter(vertices, out area);
    }

    static public Vector2 GetMassCenter(Vector2[] vertices, out float area) {
        Edge[] egdes = GetEdges(vertices);
        return GetMassCenter(egdes, out area);
    }

    static public float Cross(ref Vector2 a, ref Vector2 b) {
        return a.x * b.y - a.y * b.x;
    }

    static public float Cross2(Vector2 a, Vector2 b) {
        return a.x * b.y - a.y * b.x;
    }

    static public Vector2 GetMassCenter(Edge[] edges, out float area) {
        float Cx = 0f;
        float Cy = 0f;
        float A = 0f;
        float xy = 0;
        Vector2 vi;
        Vector2 vi_1;

        for (int i = 0; i < edges.Length; i++) {
            vi = edges[i].p1;
            vi_1 = edges[i].p2;
            xy = vi.x * vi_1.y - vi_1.x * vi.y;
            A += xy;
            Cx += (vi.x + vi_1.x) * xy;
            Cy += (vi.y + vi_1.y) * xy;
        }

        area = Mathf.Abs(A) / 2f;

        A *= 3f;
        Cx /= A;
        Cy /= A;

        Vector2 center = new Vector2(Cx, Cy);
        return center;
    }

    /// <summary>
    /// Returns edges formed by given vertices, closed. 
    /// Order: 0-1, 1-2, ..., last-0
    /// </summary>
    static public Edge[] GetEdges(Vector2[] vertices) {
        Edge[] edges = new Edge[vertices.Length];
        for (int i = 0; i < vertices.Length - 1; i++) {
            edges[i] = new Edge(vertices[i], vertices[i + 1]);
        }
        edges[vertices.Length - 1] = new Edge(vertices[vertices.Length - 1], vertices[0]);
        return edges;
    }

    public static void ShiftVertices(Vector2[] vertices, Vector2 offset) {
        for (int i = 0; i < vertices.Length; i++) {
            vertices[i] += offset;
        }
    }

    public static void ScaleVertices(Vector2[] vertices, float scale) {
        for (int i = 0; i < vertices.Length; i++) {
            vertices[i] *= scale;
        }
    }

    //TODO: refactor
    public static Vector2[] ScaleVertices2(Vector2[] vertices, float scale) {
        Vector2[] vertices2 = new Vector2[vertices.Length];
        for (int i = 0; i < vertices.Length; i++) {
            vertices2[i] = vertices[i] * scale;
        }
        return vertices2;
    }

    public static Vector2[] OffsetVerticesFromCenter(Vector2[] circulatedVertices, float offset) {
        List<Vector2> vrt = new List<Vector2>();

        for (int i = 0; i < circulatedVertices.Length - 2; i++) {
            Vector2 a = circulatedVertices[i + 1] - circulatedVertices[i];
            Vector2 b = circulatedVertices[i + 2] - circulatedVertices[i + 1];
            float rotate = Math2d.Cross(ref a, ref b);
            var outDir = (a.normalized - b.normalized) * Mathf.Sign(-rotate);
            if (Mathf.Approximately(outDir.sqrMagnitude, 0f)) {
                continue;
            } else {
                vrt.Add(circulatedVertices[i + 1] + outDir.normalized * offset);
            }
        }

        return vrt.ToArray();
    }

    static public Vector2[] RotateVerticesRad(Vector2[] vertices, float angle) {
        float cosA = Mathf.Cos(angle);
        float sinA = Mathf.Sin(angle);
        Vector2[] verticesRotated = new Vector2[vertices.Length];

        for (int i = 0; i < verticesRotated.Length; i++) {
            verticesRotated[i] = RotateVertex(vertices[i], cosA, sinA);
        }

        return verticesRotated;
    }

	//rotate conter-clockwise
    static public Vector2 RotateVertexDeg(Vector2 v, float alpha) {
        return RotateVertex(v, alpha * Mathf.Deg2Rad);
    }

	//rotate conter-clockwise
    static public Vector2 RotateVertex(Vector2 v, float alpha) {
        var cosA = Mathf.Cos(alpha);
        var sinA = Mathf.Sin(alpha);
        return new Vector2(v.x * cosA - v.y * sinA, v.x * sinA + v.y * cosA);
    }

	//rotate conter-clockwise
    static public Vector2 RotateVertex(Vector2 v, float cosA, float sinA) {
        return new Vector2(v.x * cosA - v.y * sinA, v.x * sinA + v.y * cosA);
    }

    static public bool ApproximatelySame(Vector2 a, Vector2 b) {
        return Mathf.Approximately(a.x, b.x) && Mathf.Approximately(a.y, b.y);
    }


    static public void PositionOnParent(Transform objTransform, Place place, Transform parentTransform, bool makeParent = false, float zOffset = 0) {
        float angle = Math2d.GetRotationRad(place.dir);
        objTransform.RotateAround(Vector3.zero, Vector3.back, -angle * Mathf.Rad2Deg);
        objTransform.position = place.pos;

        angle = Math2d.GetRotationRad(parentTransform.right);
        objTransform.RotateAround(Vector3.zero, Vector3.back, -angle * Mathf.Rad2Deg);
        objTransform.position += parentTransform.position;

        if (makeParent) {
            objTransform.parent = parentTransform;
        }
        objTransform.position += new Vector3(0, 0, zOffset);
    }

    static public Edge RotateEdge(Edge e, float cosA, float sinA) {
        return new Edge(RotateVertex(e.p1, cosA, sinA), RotateVertex(e.p2, cosA, sinA));
    }

    static public float RandomSign() {
        return Mathf.Sign(UnityEngine.Random.Range(-1f, 1f));
    }

    /*
	static public void TestRefDotProduct()
	{
		Vector2 a = new Vector2(2f,30f);
		Vector2 b = new Vector2(7f,12f);

		Stopwatch sw = Stopwatch.StartNew();

		for(int i = 0; i < 10000000; i++)
		{
			DotProduct(ref a, ref b);
		}

		sw.Stop();
		UnityEngine.Debug.LogError("TestRefDotProduct Time taken: " + sw.Elapsed.TotalMilliseconds);
	}

	static public void TestDotProduct()
	{
		Vector2 a = new Vector2(2f,30f);
		Vector2 b = new Vector2(7f,12f);

		Stopwatch sw = Stopwatch.StartNew();

		for(int i = 0; i < 10000000; i++)
		{
			DotProduct(a, b);
		}

		sw.Stop();
		UnityEngine.Debug.LogError("TestDotProduct Time taken: " + sw.Elapsed.TotalMilliseconds);
	}
	*/

    public static Vector2 SetX(this Vector2 v, float x) {
        return new Vector2(x, v.y);
    }

    public static Vector2 SetY(this Vector2 v, float y) {
        return new Vector2(v.x, y);
    }


    public static Vector3 SetX(this Vector3 v, float x) {
        return new Vector3(x, v.y, v.z);
    }

    public static Vector3 SetY(this Vector3 v, float y) {
        return new Vector3(v.x, y, v.z);
    }

    public static Vector3 SetZ(this Vector3 v, float z) {
        return new Vector3(v.x, v.y, z);
    }

    static public List<ParticleSystemsData> Clone(this List<ParticleSystemsData> effects) {
        return effects.ConvertAll(e => e.Clone());
    }
}


