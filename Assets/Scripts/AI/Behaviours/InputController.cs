using UnityEngine;
using System.Collections;

public interface InputController : IFreezble
{
	void Tick (float delta);
	Vector2 turnDirection{ get;}
	bool shooting{get;}
	bool accelerating{ get;}
	float accelerateValue01{ get;}
	bool braking{get;}
    void SetSpawnParent(PolygonGameObject prnt);
}

public class StaticInputController : InputController
{
    Vector2 turnDir = Vector2.zero;
    public void Tick (float delta) {}
	public Vector2 turnDirection{ get { return turnDir; } set { turnDir = value; } }
	public bool shooting{ get; set; }
	public bool accelerating{ get; set; }
	public bool braking{ get; set; }
	public void SetSpawnParent(PolygonGameObject prnt){}
	public void Freeze(float m){ }
	public float accelerateValue01{ get{ return 1f;}} 
}
