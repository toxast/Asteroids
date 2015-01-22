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

		float dangle = DeltaAngle(aimAngle);
		
		if(Mathf.Abs(dangle) <= deltaAngle)
		{
			transform.rotation = Quaternion.Euler(currentAngles.SetZ(aimAngle));
		}
		else
		{
			deltaAngle *= Mathf.Sign(dangle);
			transform.rotation = Quaternion.Euler(currentAngles + new Vector3(0, 0, deltaAngle));
		}
	}

	public float DeltaAngle(float toAngle)
	{
		return Math2d.DeltaAngleGRAD (transform.eulerAngles.z, toAngle);
	}
}
