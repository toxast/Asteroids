using UnityEngine;
using System.Collections;

public interface InputController
{
	void Tick (PolygonGameObject p);
	Vector2 turnDirection{ get;}
	bool shooting{get;}
	bool accelerating{ get;}
	bool braking{get;}
    void SetSpawnParent(PolygonGameObject prnt);
}

public class EmptyInputController : InputController
{
	public void Tick (PolygonGameObject p){}
	public Vector2 turnDirection{ get { return Vector2.zero;}}
	public bool shooting{get { return false;}}
	public bool accelerating{get { return false;}}
	public bool braking{get { return false;}}
	public void SetSpawnParent(PolygonGameObject prnt){}
}
