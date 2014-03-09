using UnityEngine;
using System.Collections;

public class TestCoroutine : MonoBehaviour {

	void Awake()
	{
		Debug.Log ("Awake 1");
		StartCoroutine (testCoroutine());
		Debug.Log ("Awake 2");
	}

	// Use this for initialization
	void Start () {
		Debug.Log ("Start");
	}
	
	// Update is called once per frame
	void Update () {
		Debug.LogError ("Update");
	}

	void FixedUpdate () {
		Debug.Log ("FixedUpdate");
	}


	private IEnumerator testCoroutine()
	{
		Debug.Log ("testCoroutine");
		while(true)
		{
			Debug.LogWarning("testCoroutine");
			yield return new WaitForSeconds(1);
		}
	}
}
