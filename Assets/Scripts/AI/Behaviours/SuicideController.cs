using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class SuicideController : BaseSpaceshipController
{
	List<IBullet> bullets;

	AIHelper.Data tickData = new AIHelper.Data();
	
	bool timeForTurnAction = false;
	float untilTurn = 0f;
	float untilTurnMax = 3.5f;
	float untilTurnMin = 1.5f;
	
	bool checkBulletsAction = false;
	float untilBulletsEvade = 1f;
	float untilBulletsEvadeMax = 3f;
	float untilBulletsEvadeMin = 1f;

	public SuicideController (SpaceShip thisShip, List<IBullet> bullets) : base(thisShip)
	{
		this.bullets = bullets;

		thisShip.StartCoroutine (Logic ());
		thisShip.StartCoroutine (BehavioursRandomTiming ());
	}

	private IEnumerator BehavioursRandomTiming()
	{
		while(true)
		{
			TickActionVariable(ref timeForTurnAction, ref untilTurn, untilTurnMin, untilTurnMax);
			TickActionVariable(ref checkBulletsAction, ref untilBulletsEvade, untilBulletsEvadeMin, untilBulletsEvadeMax);
			yield return null;
		}
	}

	private IEnumerator Logic()
	{
		shooting = false;
		accelerating = true;

		float checkBehTimeInterval = 0.1f;
		float checkBehTime = 0;
		bool behaviourChosen = false;

		float accuracy = 0.5f;
		float duration;
		Vector2 newDir;
		while(true)
		{
			if(!Main.IsNull(target))
			{
				behaviourChosen = false;
				checkBehTime -= Time.deltaTime;
				tickData.Refresh(thisShip, target);
				if(checkBehTime <= 0)
				{
					checkBehTime = checkBehTimeInterval;
					
					if (!behaviourChosen && checkBulletsAction)
					{
						if(AIHelper.EvadeBullets(thisShip, bullets, out duration, out newDir))
						{
							behaviourChosen = true;
							yield return thisShip.StartCoroutine(SetFlyDir(newDir, duration));
						}
						checkBulletsAction = false;
						if(!behaviourChosen)
						{
							untilBulletsEvade = untilBulletsEvadeMin;
						}
					}
					
					if(!behaviourChosen && timeForTurnAction)
					{
						behaviourChosen = true;
						float angle = UnityEngine.Random.Range (30, 70);
						newDir = Math2d.RotateVertex (tickData.dirNorm, -tickData.evadeSign * angle * Mathf.Deg2Rad);
						duration =  1f;
						yield return thisShip.StartCoroutine(SetFlyDir(newDir, duration)); 
						timeForTurnAction = false;
					}

					if(!behaviourChosen)
					{
						var aimVelocity = (target.velocity* 1.5f - thisShip.velocity) * accuracy;
						AimSystem aim = new AimSystem (target.position, aimVelocity, thisShip.position, thisShip.maxSpeed);  
						turnDirection = aim.direction;
						yield return null;
					}
				}
			}
			else
			{
				Brake();
				yield return new WaitForSeconds(0.5f); 
				checkBehTime -= 0.5f;
			}
		}
	}
}
