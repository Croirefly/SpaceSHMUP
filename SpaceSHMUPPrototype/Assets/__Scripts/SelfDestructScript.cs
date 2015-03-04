using UnityEngine;
using System.Collections;

public class SelfDestructScript : MonoBehaviour {
	
	public float destructTimer = 0.5f;
	
	// Use this for initialization
	void Start () {
		Destroy (gameObject, destructTimer);
	}

	// Update is called once per frame
	void Update () {
	
	}
}
