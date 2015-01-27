using UnityEngine;
using System.Collections;

public class SpaceshipData
{
	public float thrust;
	public float maxSpeed;
	public float turnSpeed;
	public float brake;
	public float passiveBrake;

	public SpaceshipData()
	{
	}

	public SpaceshipData(float thrust, float maxSpeed, float turnSpeed, float brake, float passiveBrake)
	{
		this.thrust = thrust;
		this.maxSpeed = maxSpeed;
		this.turnSpeed = turnSpeed;
		this.brake = brake;
		this.passiveBrake = passiveBrake;
	}
}
