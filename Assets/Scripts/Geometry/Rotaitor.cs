using UnityEngine;
using System.Collections;


public class Rotaitor : IFreezble
{
	private Transform transform;
	public float rotatingSpeed{ private set; get;}

	/// <summary>
	/// rotatingSpeed in angles/second
	/// </summary>
	public Rotaitor(Transform transform, float rotatingSpeed)
	{
		this.transform = transform;
		this.rotatingSpeed = rotatingSpeed;
	}

	public void Freeze(float multipiler){
		rotatingSpeed *= multipiler;
	}

	/// <summary>
	/// Rotates passed transform in direction of aimAngle by shortes arc
	/// aimAngle should be within [0, 360]
	/// returns true if rotated on desired aimAngle
	/// </summary>
	public bool Rotate(float dtime, float aimAngle)
	{
		float deltaAngle = dtime * rotatingSpeed;
		Vector3 currentAngles = transform.eulerAngles;

		float dangle = DeltaAngle(aimAngle);
		
		if(Mathf.Abs(dangle) <= deltaAngle)
		{
			transform.rotation = Quaternion.Euler(currentAngles.SetZ(aimAngle));
			return true;
		}
		else
		{
			deltaAngle *= Mathf.Sign(dangle);
			transform.rotation = Quaternion.Euler(currentAngles + new Vector3(0, 0, deltaAngle));
			return false;
		}
	}

	public void Rotate(float dtime, bool clockwise){
		float deltaAngle = dtime * rotatingSpeed;
		if (clockwise) {
			deltaAngle = -deltaAngle;
		}
		transform.rotation = Quaternion.Euler(transform.eulerAngles + new Vector3(0, 0, deltaAngle));
	}

	public float DeltaAngle(float toAngle)
	{
		return Math2d.DeltaAngleDeg (transform.eulerAngles.z, toAngle);
	}
}
