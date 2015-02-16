using UnityEngine;
using System.Collections;

public interface IBullet : IPolygonGameObject
{
	float damage{ get; set;}
	bool Expired();
}
