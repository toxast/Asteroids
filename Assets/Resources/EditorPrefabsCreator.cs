using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

public class EditorPrefabsCreator : MonoBehaviour 
{
	[MenuItem ("MyMenu/Action_Guns")]
	public static void CreateGunsPrefabs() 
	{
		var data = GunsResources.Instance.guns;
		int indx = 0;
		foreach(var dataObj in data)
		{
			var path = 	"Assets/Resources/AllDataPrefabs/Guns/Gun" + indx.ToString() + "_" + dataObj.name + ".prefab";
			indx++;

			var newComp = CreatePrefab<MGunData>(path);

			newComp.storeName = dataObj.name;
			newComp.price = dataObj.iprice;
			newComp.etype = dataObj.itype;

			newComp.damage = dataObj.damage;
			newComp.lifeTime = dataObj.lifeTime;
			newComp.bulletSpeed = dataObj.bulletSpeed;
			newComp.fireInterval = dataObj.fireInterval;
			newComp.physical = dataObj.physical.Clone();
			newComp.repeatCount = dataObj.repeatCount;
			newComp.repeatInterval = dataObj.repeatInterval;
			newComp.vertices = dataObj.vertices.ToList().ToArray();
			newComp.color = dataObj.color;
			newComp.fireEffect = dataObj.fireEffect;

			MGunsResources.Instance.guns.Add (newComp);
		}
	}

	[MenuItem ("MyMenu/Action_RocketGuns")]
	public static void CreateRocketGunsPrefabs() 
	{
		var data = GunsResources.Instance.rocketLaunchers;
		int indx = 0;
		foreach(var dataObj in data)
		{
			var path = 	"Assets/Resources/AllDataPrefabs/Guns/RocketGun" + indx.ToString() + "_" + dataObj.name + ".prefab";
			indx++;

			var newComp = CreatePrefab<MRocketGunData>(path);

			newComp.storeName = dataObj.name;
			newComp.price = dataObj.iprice;
			newComp.etype = dataObj.itype;

			newComp.damage = dataObj.baseData.damage;
			newComp.lifeTime = dataObj.baseData.lifeTime;
			newComp.fireInterval = dataObj.baseData.fireInterval;
			newComp.physical = dataObj.baseData.physical.Clone();
			newComp.repeatCount = dataObj.baseData.repeatCount;
			newComp.repeatInterval = dataObj.baseData.repeatInterval;
			newComp.vertices = dataObj.baseData.vertices.ToList().ToArray();
			newComp.color = dataObj.baseData.color;
			newComp.fireEffect = dataObj.baseData.fireEffect;
			newComp.overrideExplosionRadius = dataObj.overrideExplosionRadius;
			newComp.missleParameters = dataObj.missleParameters;
			newComp.accuracy = dataObj.accuracy;
			newComp.thrusterEffect = dataObj.thrusterEffect;
			newComp.thrusterPos = dataObj.thrusterPos;
			newComp.launchDirection = dataObj.launchDirection;
			newComp.launchSpeed = dataObj.launchSpeed;

			MGunsResources.Instance.rocketLaunchers.Add (newComp);
		}
	}

	[MenuItem ("MyMenu/Action_LazerGuns")]
	public static void CreateLazerGunsPrefabs() 
	{
		var data = GunsResources.Instance.lazerGuns;
		int indx = 0;
		foreach(var dataObj in data)
		{
			var path = 	"Assets/Resources/AllDataPrefabs/Guns/LazerGun" + indx.ToString() + "_" + dataObj.name + ".prefab";
			indx++;

			var newComp = CreatePrefab<MLazerGunData>(path);

			newComp.storeName = dataObj.name;
			newComp.price = dataObj.iprice;
			newComp.etype = dataObj.itype;

			newComp.damage = dataObj.baseData.damage;
			newComp.attackDuration = dataObj.baseData.lifeTime;
			newComp.pauseDuration = dataObj.baseData.fireInterval;
			newComp.color = dataObj.baseData.color;
			newComp.fireEffect = dataObj.baseData.fireEffect;
			newComp.distance = dataObj.distance;
			newComp.width = dataObj.width;

			MGunsResources.Instance.lazerGuns.Add (newComp);
		}
	}

	[MenuItem ("MyMenu/Action_SpawnerGuns")]
	public static void CreateSpawnerPrefabs() 
	{
		var data = GunsResources.Instance.spawnerGuns;
		int indx = 0;
		foreach(var dataObj in data)
		{
			var path = 	"Assets/Resources/AllDataPrefabs/Guns/SpawnGun" + indx.ToString() + "_" + dataObj.name + ".prefab";
			indx++;

			var newComp = CreatePrefab<MSpawnerGunData>(path);

			newComp.storeName = dataObj.name;
			newComp.price = dataObj.iprice;
			newComp.etype = dataObj.itype;

			newComp.bulletSpeed = dataObj.baseData.bulletSpeed;
			newComp.fireInterval = dataObj.baseData.fireInterval;
			newComp.fireEffect = dataObj.baseData.fireEffect;
			newComp.maxSpawn = dataObj.maxSpawn;
			newComp.startSpawn = dataObj.startSpawn;
			newComp.startSpawnInterval = dataObj.startSpawnInterval;

			MGunsResources.Instance.spawnerGuns.Add (newComp);
		}
	}

	[MenuItem ("MyMenu/Action_Turrets")]
	public static void CreateTurretsPrefabs() 
	{
		var data = SpaceshipsResources.Instance.turrets;
		int indx = 0;
		foreach(var dataObj in data)
		{
			var path = 	"Assets/Resources/AllDataPrefabs/SpaceShips/Turret_" +indx.ToString() + "_" + dataObj.name + ".prefab";
			indx++;

			var newComp = CreatePrefab<MTurretData>(path);

			newComp.color = dataObj.color;
			newComp.rotationSpeed = dataObj.rotationSpeed;
			newComp.restrictionAngle = dataObj.restrictionAngle;
			newComp.color = dataObj.color; 
			newComp.repeatTargetCheck = dataObj.repeatTargetCheck; 
			newComp.guns = GetGuns(dataObj.guns);
			newComp.linkedGuns =  new List<int>(dataObj.linkedGuns);
			newComp.verts = dataObj.verts.ToList().ToArray(); 

			MSpaceShipResources.Instance.turrets.Add (newComp);
		}
	}

	[MenuItem ("MyMenu/Action_Towers")]
	public static void CreateTwPrefabs() 
	{
		MSpaceShipResources.Instance.towers.Clear ();

		var data = SpaceshipsResources.Instance.towers;
		int indx = 0;
		foreach(var dataObj in data)
		{
			var path = 	"Assets/Resources/AllDataPrefabs/SpaceShips/Tower" +indx.ToString() + "_" + dataObj.name + ".prefab";
			indx++;

			var newComp = CreatePrefab<MTowerData>(path);

			newComp.reward = dataObj.reward;
			newComp.physical = dataObj.physical.Clone(); 
			newComp.color = dataObj.color; 
			newComp.shootAngle = dataObj.shootAngle;
			newComp.rotationSpeed = dataObj.rotationSpeed;
			newComp.repeatTargetCheck = dataObj.repeatTargetCheck;
			newComp.accuracy = dataObj.accuracy.Clone();
			newComp.shield = dataObj.shield.Clone(); 
			newComp.guns = GetGuns (dataObj.guns);
			newComp.linkedGuns = new List<int> (dataObj.linkedGuns);
			newComp.verts = dataObj.verts.ToList ().ToArray ();
			newComp.turrets = GetTurrets (dataObj.turrets);

			MSpaceShipResources.Instance.towers.Add (newComp);
		}
	}

	[MenuItem ("MyMenu/Action_Spaceships")]
	public static void CreateSpPrefabs() 
	{
		MSpaceShipResources.Instance.spaceships.Clear ();

		var data = SpaceshipsResources.Instance.spaceships;
		int indx = 0;
		foreach(var dataObj in data)
		{
			var path = 	"Assets/Resources/AllDataPrefabs/SpaceShips/Ship" +indx.ToString() + "_" + dataObj.name + ".prefab";
			indx++;

			var newComp = CreatePrefab<MSpaceshipData>(path);

			newComp.price = dataObj.price;
			newComp.reward = dataObj.reward;
			newComp.ai = dataObj.ai;
			newComp.color = dataObj.color; 
			newComp.physical = dataObj.physical.Clone(); 
			newComp.mobility = dataObj.mobility.Clone();
			newComp.accuracy = dataObj.accuracy.Clone();
			newComp.shield = dataObj.shield.Clone(); 

			newComp.guns = GetGuns (dataObj.guns);
			newComp.linkedGuns = new List<int> (dataObj.linkedGuns);
			newComp.thrusters = dataObj.thrusters.ConvertAll(t => t.Clone());
			newComp.turrets = GetTurrets (dataObj.turrets);
			newComp.verts = dataObj.verts.ToList ().ToArray ();
			newComp.upgradeIndex = dataObj.upgradeIndex;

			MSpaceShipResources.Instance.spaceships.Add (newComp);
		}
	}

	private static List<MGunSetupData> GetGuns(List<GunSetupData> guns)
	{
		List<MGunSetupData> result = new List<MGunSetupData> ();
		foreach (var item in guns) {

			MGunBaseData mgun = null;

			if (item.type == GunSetupData.eGuns.BULLET) {
				mgun = MGunsResources.Instance.guns [item.index];
			} else if (item.type == GunSetupData.eGuns.ROCKET) {
				mgun = MGunsResources.Instance.rocketLaunchers [item.index];
			} else if (item.type == GunSetupData.eGuns.LAZER) {
				mgun = MGunsResources.Instance.lazerGuns [item.index];
			} else if (item.type == GunSetupData.eGuns.SPAWNER) {
				mgun = MGunsResources.Instance.spawnerGuns [item.index];
			} else {
				Debug.LogError ("WTf is that gun: " + item.type);
			}

			if (mgun != null) {
				MGunSetupData setup = new MGunSetupData {
					gun = mgun,
					place = item.place.Clone ()
				};

				result.Add (setup);
			}
		}

		return result;
	}

	private static List<MTurretReferenceData> GetTurrets(List<TurretReferenceData> oldObjs)
	{
		List<MTurretReferenceData> result = new List<MTurretReferenceData> ();
		foreach (var item in oldObjs) {

			MTurretData newObj = null;

			try
			{
				newObj = MSpaceShipResources.Instance.turrets[item.index];
			}
			catch {
				Debug.LogError ("error turrets: " + item.index);
			}
			finally {

				if (newObj != null) {
					MTurretReferenceData setup = new MTurretReferenceData {
						turret = newObj,
						place = item.place.Clone ()
					};

					result.Add (setup);
				}
			}
		}

		return result;
	}


	public static T CreatePrefab<T>(string path)
		where T: Component
	{
		var newPrefab = PrefabUtility.CreateEmptyPrefab (path);
		var newGO = new GameObject ();
		var newPrefabGameobject = PrefabUtility.ReplacePrefab (newGO, newPrefab);
		DestroyImmediate (newGO);
		var newComp = newPrefabGameobject.AddComponent<T>();
		EditorUtility.SetDirty (newPrefabGameobject);
		return newComp;
	}


}
