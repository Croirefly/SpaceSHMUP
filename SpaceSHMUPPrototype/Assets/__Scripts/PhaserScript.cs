using UnityEngine;
using System.Collections;

public class PhaserScript : MonoBehaviour {
	public float waveFrequency = 2;
	public float speed = 20f;
	private Vector3 pos;
	private float birthTime;

	public float waveWidth = 4;
	public float waveRotY = 45;
	
	private float x0 = -12345;
	// Use this for initialization
	void Start () {
		pos=this.transform.position;
		birthTime = Time.time;
		x0 = pos.x;
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 tempPos = this.transform.position;
		float age = Time.time - birthTime;
		float theta = Mathf.PI * 2 * age/waveFrequency; 
		float sin = Mathf.Sin (theta);
		tempPos.x = x0 + waveWidth * sin;
		pos = tempPos;
		Vector3 rot = new Vector3 (0, sin*waveRotY, 0);


		tempPos.y += speed * Time.deltaTime;
		this.transform.position = tempPos;
	}
}
