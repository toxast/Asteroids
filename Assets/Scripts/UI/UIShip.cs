﻿using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class UIShip : MonoBehaviour 
{
	[SerializeField] Camera cam;
	[SerializeField] Transform spaceshipHolder;
	SpaceShip spaceship;

	public void Create(MSpaceshipData data) {
		Clear ();
		spaceship = PolygonCreator.CreatePolygonGOByMassCenter<SpaceShip> (data.verts, data.color);
		spaceship.InitPolygonGameObject (data.physical);
		spaceship.SetThrusters (data.thrusters);
		spaceship.ShowFullThruster ();
		spaceship.cacheTransform.parent = spaceshipHolder;
		spaceship.cacheTransform.localPosition = Vector3.zero;
		Debug.LogWarning ("created " + data.name + " health:" + spaceship.fullHealth + " area:" + spaceship.polygon.area + " R:" + spaceship.polygon.R);
	}

	public void Clear() {
		if (spaceship != null) {
			Destroy (spaceship.gameObject);
		}
	}
}
