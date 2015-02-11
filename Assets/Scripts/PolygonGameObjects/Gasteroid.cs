using UnityEngine;
using System.Collections;

public class Gasteroid : Asteroid
{
	protected override float healthModifier {
		get {
			return 0.3f;
		}
	}

}
