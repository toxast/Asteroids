using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spliter 
{
	public static List<Asteroid> SplitIntoAsteroids(IPolygonGameObject polygonGo)
	{
		List<Vector2[]> polys = polygonGo.Split();
		List<Asteroid> parts = new List<Asteroid>();
		
		if(polys.Count < 2)
		{
			Debug.LogError("couldnt split asteroid");
		}
		
		foreach(var vertices in polys)
		{
			Asteroid asteroidPart = PolygonCreator.CreatePolygonGOByMassCenter<Asteroid>(vertices, polygonGo.GetColor(), polygonGo.meshUV);
			
			//			if(PolygonCreator.CheckIfVerySmallOrSpiky(asteroidPart.polygon))
			//			{
			//				Destroy(asteroidPart.gameObject);
			//			}
			//			else
			//			{
			asteroidPart.Init();
			asteroidPart.cacheTransform.Translate(polygonGo.position);
			asteroidPart.cacheTransform.RotateAround(polygonGo.cacheTransform.position, -Vector3.back, polygonGo.cacheTransform.rotation.eulerAngles.z);
			asteroidPart.gameObject.name = "asteroid part";
			
			parts.Add(asteroidPart);
			//}
		}
		
		CalculateObjectPartVelocity(parts, polygonGo);
		
		return parts;
	}

	private static void CalculateObjectPartVelocity(List<Asteroid> parts, IPolygonGameObject mainPart)
	{
		Vector2 mainVelocity = mainPart.velocity;
		float mainRotation = mainPart.rotation* Math2d.PIdiv180; 
		
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
			distances.Add(parts[i].cacheTransform.position - mainPart.cacheTransform.position);
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
				part.velocity += (Vector3)mainVelocity.normalized * Mathf.Sqrt( 2f * pieceInertiaEnergy / part.mass );
			}
			
			float pieceRotationEnergy = mainPartRotationEnergy * (rotationWeights[i]/ sumRotationWeights);
			float velocityEnegryFromRotation = kRotationEnergyToVelocity * pieceRotationEnergy;
			pieceRotationEnergy = pieceRotationEnergy * (1 - kRotationEnergyToVelocity);
			
			part.rotation = rotationSign * Mathf.Sqrt( 2f * pieceRotationEnergy / part.inertiaMoment ) / Math2d.PIdiv180;
			
			Vector2 perpendecular = new Vector2(direction.y, -direction.x);
			part.velocity += (Vector3)perpendecular.normalized * rotationSign * Mathf.Sqrt( 2f * velocityEnegryFromRotation / part.mass );
			
			part.velocity = part.velocity.SetZ(0);//TODO: z system*/
		}
	}
}
