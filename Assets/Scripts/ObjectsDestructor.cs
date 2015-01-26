using UnityEngine;
using System.Collections;

public class ObjectsDestructor
{

	public GameObject g;
	public float initialTime;
	public float timeLeft;
	
	public ObjectsDestructor(GameObject g, float timeLeft)
	{
		this.g = g;
		this.initialTime = timeLeft;
		this.timeLeft = initialTime;
	}
	
	public void Tick(float dtime)
	{
		timeLeft -= dtime;
	}
	
	public bool IsTimeExpired()
	{
		return timeLeft <= 0;
	}
}
