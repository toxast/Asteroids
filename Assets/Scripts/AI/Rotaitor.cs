using UnityEngine;
using System.Collections;


public class Rotaitor
{
	private Transform transform;
	private float rotatingSpeed;

	/// <summary>
	/// rotatingSpeed in angles/second
	/// </summary>
	public Rotaitor(Transform transform, float rotatingSpeed)
	{
		this.transform = transform;
		this.rotatingSpeed = rotatingSpeed;
	}

	/// <summary>
	/// Rotates passed transform in direction of aimAngle by shortes arc
	/// aimAngle should be within [0, 360]
	/// </summary>
	public void Rotate(float dtime, float aimAngle)
	{
		float deltaAngle = dtime * rotatingSpeed;
		Vector3 currentAngles = transform.eulerAngles;

		float diff = aimAngle - currentAngles.z;
		
		if(Mathf.Abs(diff) <= deltaAngle)
		{
			transform.rotation = Quaternion.Euler(currentAngles.SetZ(aimAngle));
		}
		else
		{
			float sign = Mathf.Sign(diff)*Mathf.Sign(180f - Mathf.Abs(diff));
			deltaAngle *= sign;
			transform.rotation = Quaternion.Euler(currentAngles + new Vector3(0, 0, deltaAngle));
		}
	}

}
