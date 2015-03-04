using UnityEngine;
using System.Collections;

//Enemy_1 extends the Enemy class

public class Enemy_1 : Enemy {
	//because Enemy_1 extends Enemy, the ___ bool wont work

	//# of sec for a full sine wave
	public float waveFrequency = 2;

	public float waveWidth = 4;
	public float waveRotY = 45;

	private float x0 = -12345;
	private float birthTime;


	// Use this for initialization
	void Start () {
		//se x0 to initial x pos of Enemy_1
		x0 = pos.x;
		birthTime = Time.time;
	}
	//override the Move function on Enemy
	public override void Move() {
		Vector3 tempPos = pos;

		float age = Time.time - birthTime;
		float theta = Mathf.PI * 2 * age/waveFrequency; 
		float sin = Mathf.Sin (theta);
		tempPos.x = x0 + waveWidth * sin;
		pos = tempPos;

		Vector3 rot = new Vector3 (0, sin*waveRotY, 0);
		this.transform.rotation = Quaternion.Euler (rot);
		base.Move ();
	}
}
