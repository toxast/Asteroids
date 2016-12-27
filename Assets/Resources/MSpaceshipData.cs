using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class MSpaceshipData : MonoBehaviour, IGotShape, IGotThrusters, IGotGuns, IGotTurrets
{
	public int price = -1;
	public int reward = 0;
	public Color color = Color.white;
	public PhysicalData physical;
	public AIType ai = AIType.eCommon;
	public SpaceshipData mobility;
	public AccuracyData accuracy;
	public ShieldData shield;
	public List<MGunSetupData> guns;
	public List<int> linkedGuns;
	public List<ThrusterSetupData> thrusters;
	public List<MTurretReferenceData> turrets;
	public Vector2[] verts;
	public int upgradeIndex;
    public bool symmetric;

	//interfaces
	public Vector2[] iverts {get {return verts;} set{verts = value;}}
	public List<MGunSetupData> iguns {get {return guns;} set{guns = value;}}
	public List<ThrusterSetupData> ithrusters {get {return thrusters;} set{thrusters = value;}}
	public List<MTurretReferenceData> iturrets {get {return turrets;} set{turrets = value;}}
    public bool isymmetric { get { return symmetric; } set { symmetric = value; } }

    void Awake()
    {
        if (Application.isPlaying && Application.isEditor) {
            var obj = Create ();
            obj.cacheTransform.position = gameObject.transform.position;

            if(obj.layerNum != CollisionLayers.ilayerAsteroids)
                obj.targetSystem = new TargetSystem (obj);

            obj.SetCollisionLayerNum (CollisionLayers.ilayerTeamEnemies);
            Singleton<Main>.inst.Add2Objects(obj);
        }
    }

    public PolygonGameObject Create()
    {
        return ObjectsCreator.MCreateSpaceShip<SpaceShip> (this);
    }
}
