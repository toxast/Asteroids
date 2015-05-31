using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class BulletAdapter : IBullet
{
	public IPolygonGameObject go;

	public float damage{ get; set;}
	public bool breakOnDeath { get; set;}
	public bool Expired()
	{
		return false;
	}

	public BulletAdapter (IPolygonGameObject go)
	{
		this.go = go;
	}

	public void SetTarget(IPolygonGameObject target)
	{
		go.SetTarget (target);
	}

	public void Tick(float delta)
	{
		go.Tick (delta);
	}

	public IPolygonGameObject target{get{return go.target;}}

	public GameObject gameObj{get{return go.gameObj;}}
	public Polygon polygon{get{return go.polygon;}}
	public Polygon globalPolygon{get{return go.globalPolygon;} set{go.globalPolygon = value;}}
	public Transform cacheTransform{get{return go.cacheTransform;}}
	public Vector2 position{get{return go.position;} set{go.position = value;}}
	public Color GetColor()
	{
		return go.GetColor ();
	}

	public Material mat{get{return go.mat;} set{go.mat = value;}}
	
	//collision
	public int layerNum{get{return go.layerNum;} set{go.layerNum = value;}}
	public int layer{get{return go.layer;} set{go.layer = value;}}
	public int collision{get{return go.collision;} set{go.collision = value;}}

	public void SetCollisionLayerNum (int layerN) {go.SetCollisionLayerNum (layerN);}
	public PolygonGameObject.DestructionType destructionType{get{return go.destructionType;} set{go.destructionType = value;}}
	public bool destroyOnBoundsTeleport{get{return go.destroyOnBoundsTeleport;} set{go.destroyOnBoundsTeleport = value;}}

	public PolygonGameObject minimapIndicator {get{return go.minimapIndicator;} set{go.minimapIndicator = value;}}

	//physical
	public float density{get{return go.density;}}
	public float healthModifier{get{return go.healthModifier;}}
	public float collisionDefence{get{return go.collisionDefence;}}
	public float collisionAttackModifier{get{return go.collisionAttackModifier;}}

	public int reward{get{return go.reward;} set{go.reward = value;}}

	public float mass{get{return go.mass;}}
	public float inertiaMoment{get{return go.inertiaMoment;}}
	public Vector2 velocity{get{return go.velocity;} set{go.velocity = value;}}
	public float rotation{get{return go.rotation;} set{go.rotation = value;}}
	public PolygonCreator.MeshDataUV meshUV{get{return go.meshUV;} set{go.meshUV = value;}}
	public void Hit(float dmg)
	{
		go.Hit (dmg);
	}
	public void Kill()
	{
		go.Kill ();
	}
	public event Action<float> healthChanged
	{
		add{go.healthChanged += value;}
		remove{go.healthChanged -= value;}
	}
	public bool IsKilled()
	{
		return go.IsKilled ();
	}
	
	public List<Gun> guns{get{return go.guns;}}
	public List<PolygonGameObject> turrets{get{return go.turrets;} set{go.turrets = value;}}
	
	public DropID dropID{get{return go.dropID;} set{go.dropID = value;}}
	
	public DeathAnimation deathAnimation{get{return go.deathAnimation;} set{go.deathAnimation = value;}}
	
	public List<Vector2[]> Split()
	{
		return go.Split();
	}
}
