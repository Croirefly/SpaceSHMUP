using UnityEngine;
using System.Collections;

public class Shield : MonoBehaviour {

	public float rotationsPerSecondZ = 0.1f;
	public float rotationsPerSecondY = 0.1f;
	public float rotationsPerSecondX = 0.1f;
	public bool __________;
	public int levelShown = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		//read the current sheild level from the singleton
		int currLevel = Mathf.FloorToInt (Hero.S.shieldLevel);
		//if this is diff from level shown
		if (levelShown != currLevel) {
			levelShown = currLevel;
			Material mat = this.GetComponent<Renderer>().material;
			//adjust the texture offset to show diff shield level
			mat.mainTextureOffset = new Vector2 (0.2f*levelShown, 0);
		}

		//rotate the shield a bit every second
		float rZ = (rotationsPerSecondZ * Time.time * 360) % 360;
		float rY = (rotationsPerSecondY * Time.time * 360) % 360;
		float rX = (rotationsPerSecondX * Time.time * 360) % 360;

		transform.rotation = Quaternion.Euler (0,rY,rZ);
	}
}
