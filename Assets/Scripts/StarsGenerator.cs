using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StarsGenerator : MonoBehaviour 
{
	public Sprite[] textures;
	public Color[] colors;

	public GameObject[] stars;

	public void Generate(int count, Rect rect, float z)
	{
		stars = new GameObject[count];
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

			GameObject g = new GameObject();
			stars[i] = g;
			g.transform.position = new Vector3(
				Random.Range(rect.xMin, rect.xMax), 
			    Random.Range(rect.yMin, rect.yMax),
				z + Random.Range(19f, 20f));

			var renderer = g.AddComponent<SpriteRenderer>();
			renderer.sprite = tex;
			renderer.color = col;

			var size = Random.Range(7, 11);
			g.transform.localScale = new Vector3(size, size, 1);

			g.transform.parent = transform;
		}
	}
}
