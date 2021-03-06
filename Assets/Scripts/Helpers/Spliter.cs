using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spliter 
{
	public static List<Asteroid> SplitIntoAsteroids(PolygonGameObject polygonGo)
	{
		List<Vector2[]> polys = polygonGo.Split();
		List<Asteroid> parts = new List<Asteroid>();
		
		if(polys.Count < 2)
		{
			Debug.LogError("couldnt split asteroid");
		}

        float overrideHealthModifier = polygonGo.healthModifier;
        //make spaceship parts weak after explosion
		if(!(polygonGo is Asteroid)) {
            overrideHealthModifier /= 2f * polygonGo.density;
        }

        foreach (var vertices in polys)
		{
			Asteroid asteroidPart = PolygonCreator.CreatePolygonGOByMassCenter<Asteroid>(vertices, polygonGo.GetColor(), polygonGo.mat, polygonGo.meshUV);
			string suffix = " destroyed part";
			asteroidPart.InitPolygonGameObject(new PhysicalData(polygonGo.density, overrideHealthModifier, polygonGo.collisionDefence, polygonGo.collisionAttackModifier));
			asteroidPart.SetLayerNum(CollisionLayers.ilayerAsteroids);
			asteroidPart.cacheTransform.Translate(polygonGo.cacheTransform.position);
			asteroidPart.cacheTransform.RotateAround(polygonGo.position, -Vector3.back, polygonGo.cacheTransform.rotation.eulerAngles.z);
			asteroidPart.priority = PolygonGameObject.ePriorityLevel.LOW;
			if (!polygonGo.name.Contains (suffix)) {
				asteroidPart.gameObject.name = polygonGo.name + suffix;
			} else {
				asteroidPart.gameObject.name = polygonGo.name + " x";
			}
			
			parts.Add(asteroidPart);
		}

		var afterlifeEffects = polygonGo.GetEffectsForSplitParts ();
		foreach (var item in afterlifeEffects) {
			if (item.system != null && item.system.transform != null) {
				var pos = item.system.transform.position;
				foreach (var part in parts) {
					if (part.globalPolygon.IsPointInside (pos)) {
						item.system.transform.SetParent (part.cacheTransform, true);
						item.system.transform.localPosition = item.system.transform.localPosition.SetZ (item.data.zOffset);
						break;
					}
				}
			}
		}
		
		CalculateObjectPartVelocity(parts, polygonGo);
		
		return parts;
	}

	private static void CalculateObjectPartVelocity(List<Asteroid> parts, PolygonGameObject mainPart)
	{
		Vector2 mainVelocity = mainPart.velocity;
		float mainRotation = mainPart.rotation* Mathf.Deg2Rad; 
		
		float mainPartEnergy = 0.5f * mainPart.mass * mainVelocity.sqrMagnitude;
		float mainPartRotationEnergy = 0.5f * mainPart.inertiaMoment * (mainRotation*mainRotation);
		
		float kInertiaToBlow = 0.1f;
		float kVelocityEnergyToRotation = 0.1f;
		float kRotationEnergyToVelocity = 0.3f;
		
		mainPartRotationEnergy += mainPartEnergy * kVelocityEnergyToRotation;
		mainPartEnergy = mainPartEnergy * (1 - kVelocityEnergyToRotation);
		
		float blowEnergy = Mathf.Sqrt(mainPart.mass) +  mainPartEnergy * kInertiaToBlow;
		float inertiaEnergy = mainPartEnergy*(1 - kInertiaToBlow);
		
		List<Vector2> distances = new List<Vector2> (parts.Count);
		for (int i = 0; i < parts.Count; i++) 
		{
			distances.Add(parts[i].position - mainPart.position);
		}
		
		float sumVelocityWeights = 0;
		List<float> velocityWeights = new List<float> (parts.Count);
		for (int i = 0; i < parts.Count; i++) 
		{
			velocityWeights.Add(Mathf.Sqrt(parts[i].mass));
			sumVelocityWeights += velocityWeights[i];
		}
		
		float sumRotationWeights = 0;
		List<float> rotationWeights = new List<float> (parts.Count);
		for (int i = 0; i < parts.Count; i++) 
		{
			rotationWeights.Add(Mathf.Sqrt(1f/parts[i].polygon.R));
			sumRotationWeights += rotationWeights[i];
		}
		
		float rotationSign = Mathf.Sign (mainRotation);
		for (int i = 0; i < parts.Count; i++) 
		{
			Asteroid part = parts[i];
			float pieceBlowEnergy = blowEnergy * (velocityWeights[i] / sumVelocityWeights); 
			float pieceInertiaEnergy = inertiaEnergy * (velocityWeights[i] / sumVelocityWeights); 
			
			Vector2 direction = distances[i];
			part.velocity = direction.normalized * Mathf.Sqrt(2f * pieceBlowEnergy / part.mass );
			if(mainVelocity != Vector2.zero)
			{
				part.velocity += mainVelocity.normalized * Mathf.Sqrt( 2f * pieceInertiaEnergy / part.mass );
			}
			
			float pieceRotationEnergy = mainPartRotationEnergy * (rotationWeights[i]/ sumRotationWeights);
			float velocityEnegryFromRotation = kRotationEnergyToVelocity * pieceRotationEnergy;
			pieceRotationEnergy = pieceRotationEnergy * (1 - kRotationEnergyToVelocity);
			
			part.rotation = rotationSign * Mathf.Sqrt( 2f * pieceRotationEnergy / part.inertiaMoment ) * Mathf.Rad2Deg;
			
			Vector2 perpendecular = new Vector2(direction.y, -direction.x);
			part.velocity += perpendecular.normalized * rotationSign * Mathf.Sqrt( 2f * velocityEnegryFromRotation / part.mass );
		}
	}
}
