using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public interface IPolygonGameObject : IGotTarget, ITickable
{
	GameObject gameObj{get;}
	Polygon polygon{get;}
	Polygon globalPolygon{get; set;}
	Transform cacheTransform{get;}
	Vector2 position{get; set;}

	//render
	Color GetColor();
	PolygonCreator.MeshDataUV meshUV{get; set;}
	Material mat{get; set;}

	IPolygonGameObject target{ get;}

	//collision
	int layerNum{get; set;}
	int layer{get; set;}
	int collision{get; set;}
	
	//physical
	float density{get;}
	float healthModifier{get;}
	float collisionDefence{ get;}
	float collisionAttackModifier{ get;}

	int reward{ get; set;}

	float mass{get;}
	float inertiaMoment{get;}
	Vector2 velocity{get; set;}
	float rotation{get; set;}

	void Hit(float dmg);
	void Kill();
	event Action<float> healthChanged;
	bool IsKilled();

	PolygonGameObject minimapIndicator { get; set;}

	bool destroyOnBoundsTeleport{get; set;}
	PolygonGameObject.DestructionType destructionType{get; set;}

	List<Gun> guns{get;}
	
	DropID dropID{get; set;}
	
	DeathAnimation deathAnimation{get; set;}

	List<Vector2[]> Split();
	List<PolygonGameObject> turrets{get; set;}

	void SetCollisionLayerNum (int layerNum);
}
