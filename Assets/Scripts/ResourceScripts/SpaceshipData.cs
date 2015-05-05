using UnityEngine;
using System.Collections;

[System.Serializable]
public class SpaceshipData : IClonable<SpaceshipData>
{
	public float thrust = 45f;
	public float maxSpeed = 20f;
	public float turnSpeed = 220f;
	public float brake = 15f;
	public float passiveBrake = 2f;

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

	public SpaceshipData Clone()
	{
		return new SpaceshipData(thrust, maxSpeed, turnSpeed, brake, passiveBrake);
	}
}
