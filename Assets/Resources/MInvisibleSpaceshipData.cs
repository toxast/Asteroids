﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MInvisibleSpaceshipData : MSpaceshipData
{
	public InvisibleData invisibleData;

	[Header("editor field")]
	[SerializeField] MSpaceshipData fillFrom;

	private void OnValidate() {
		if (fillFrom != null) {
//			this.accuracy = fillFrom.accuracy;
//			this.color = fillFrom.color;
//			this.verts = fillFrom.verts;

			System.Type type = fillFrom.GetType();
			Component copy = this;
			// Copied fields can be restricted with BindingFlags
			System.Reflection.FieldInfo[] fields = type.GetFields(); 
			foreach (System.Reflection.FieldInfo field in fields)
			{
				field.SetValue(copy, field.GetValue(fillFrom));
			}

			fillFrom = null;
		}
	}

	public override SpaceShip Create()
	{
		return ObjectsCreator.CreateInvisibleSpaceship<SpaceShip>(this, editorSpawnLayer);
	}

}

[System.Serializable]
public class InvisibleData
{
	public float attackDutation = 3f;
	public float invisibleDuration = 4f;
	public float fadeOutDuration = 1f; 
	public float fadeInDuration = 0.6f; 
}