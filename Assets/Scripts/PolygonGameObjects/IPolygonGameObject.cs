using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public interface IPolygonGameObject : IGotTarget, ITickable
{
	GameObject gameObj{get;}
	Polygon polygon{get;}
	Transform cacheTransform{get;}
	Vector2 position{get; set;}
	Color GetColor();
	PolygonCreator.MeshDataUV meshUV{ get; set;}
	//collision
	int layer{get; set;}
	int collision{get; set;}
	
	//physical
	float density{get;}
	float mass{get;}
	float inertiaMoment{get;}
	Vector3 velocity{get; set;}
	float rotation{get; set;}

	void Hit(float dmg);
	void Kill();
	event Action<float> healthChanged;
	bool IsKilled();

	List<Gun> guns{get; set;}
	
	DropID dropID{get; set;}
	
	DeathAnimation deathAnimation{get; set;}

	List<Vector2[]> Split();
}
