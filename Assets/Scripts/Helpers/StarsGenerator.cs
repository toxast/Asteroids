using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StarsGenerator : MonoBehaviour 
{
	[SerializeField] SpriteRenderer starPrefab;
	public Sprite[] textures;
	public Color[] colors;

	public Transform[] stars;

	void Awake(){
		starPrefab.CreatePool (20);
	}

	public void Generate(int count, Rect rect, float z)
	{
		stars = new Transform[count];
		for (int i = 0; i < count; i++) 
		{
			var tex = textures[Random.Range(0, textures.Length)];
			Color col;
			if(Random.Range(0f, 1f) > 0.7f)
			{
				col = Color.white;
			}
			else
			{
				col = colors[Random.Range(0, colors.Length)];
			}

			var renderer =  starPrefab.Spawn ();
			GameObject g = renderer.gameObject;
			Transform t = g.transform;
			stars[i] = t;
			t.position = new Vector3(
				Random.Range(rect.xMin, rect.xMax), 
			    Random.Range(rect.yMin, rect.yMax),
				z + Random.Range(19f, 20f));

			renderer.sprite = tex;
			renderer.color = col;

			var size = Random.Range(7, 11);
			t.localScale = new Vector3(size, size, 1);

			t.parent = transform;
		}
	}

	public void Clear()
	{
		foreach (var s in stars) {
			s.gameObject.Recycle ();
		}
		stars = null;
	}
}
