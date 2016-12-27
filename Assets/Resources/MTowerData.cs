using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MTowerData : MonoBehaviour, IGotShape, IGotGuns, IGotTurrets
{
	public int reward;
	public PhysicalData physical;
	public Color color = Color.white;
	public float shootAngle = 20;
	public float rotationSpeed = 50;
	public float repeatTargetCheck = 1f;
	public AccuracyData accuracy;
	public ShieldData shield;
	public List<MGunSetupData> guns;
	public List<int> linkedGuns;
	public List<MTurretReferenceData> turrets;
	public Vector2[] verts;
    public bool symmetric;

    //interfaces
    public Vector2[] iverts {get {return verts;} set{verts = value;}}
	public List<MGunSetupData> iguns {get {return guns;} set{guns = value;}}
	public List<MTurretReferenceData> iturrets {get {return turrets;} set{turrets = value;}}
    public bool isymmetric { get { return symmetric; } set { symmetric = value; } }

    void Awake()
    {
        if (Application.isPlaying && Application.isEditor) {
            var obj = Create ();
            obj.cacheTransform.position = gameObject.transform.position;

            if(obj.layerNum != CollisionLayers.ilayerAsteroids)
                obj.targetSystem = new TargetSystem (obj);

            Singleton<Main>.inst.Add2Objects(obj);
        }
    }

    public PolygonGameObject Create()
    {
        return ObjectsCreator.CreateSimpleTower(this);
    }
}
