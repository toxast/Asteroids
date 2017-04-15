using UnityEngine;
using System.Collections;

[System.Serializable]
public class Place: IClonable<Place>
{
	public Vector2 pos = Vector2.zero;
	public Vector2 dir = new Vector2(1,0);
	public bool useAngleForPosition = false;
	public float range;
	public float angle;

	public Vector2 position{
		get{ 
			if (!useAngleForPosition) {
				return pos;
			} else {
				return Math2d.RotateVertexDeg (new Vector2 (range, 0), angle);
			}
		}
	}

	public Vector2 direction{
		get{ 
			if (!useAngleForPosition) {
				return dir;
			} else {
				return Math2d.RotateVertexDeg (new Vector2 (1, 0), angle);
			}
		}
	}

	public float radians{
		get{
			if (!useAngleForPosition) {
				return Math2d.GetRotationRad (dir);
			} else {
				return Mathf.Deg2Rad * angle;
			}
		}
	}

	public Place(){}
	public Place(Vector2 pos, Vector2 dir)
	{
		this.pos = pos;
		this.dir = dir;
	}

	public Place Clone()
	{
		return new Place (pos, dir);
	}

}
