using UnityEngine;
using System.Collections;

public class Enemy_2 : Enemy {

	public Vector3[] points;
	public float birthTime;
	public float lifeTime = 10;

	public float sinEccentricity = 0.6f; //how much the sine wave will affect movement

	// Use this for initialization
	void Start () {
		points = new Vector3[2];

		//find Utils.cambounds
		Vector3 cbMin = Utils.camBounds.min;
		Vector3 cbMax = Utils.camBounds.max;

		Vector3 v = Vector3.zero;
		//pick any point on the left side of the screen
		v.x = cbMin.x - Main.S.enemySpawnPadding;
		v.y = Random.Range (cbMin.y, cbMax.y);
		points[0] = v;

		//pick any point on the right side of the screen
		v = Vector3.zero;
		v.x = cbMin.x + Main.S.enemySpawnPadding;
		v.y = Random.Range (cbMin.y, cbMax.y);
		points[1] = v;

		//possible swap sides
		if(Random.value < 0.5f) {
			//setting the .x of each point to its negative wil move it to the others ide of the screen
			points[0].x *= -1;
			points[1].x *= -1;
		}

		birthTime = Time.time;
	}

	public override void Move () {
		float u = (Time.time - birthTime) / lifeTime;
		//if u> 1, then it has been longer than lifetime since brith time
		if (u > 1) {
			Destroy(this.gameObject);
			return;
		}

		//adjust u by adding an easing curve vased on a sine wave
		u = u + sinEccentricity*(Mathf.Sin(u*Mathf.PI*2));

		//interpolate the two linear interpolation points
		pos = (1-u)*points[0] + u*points[1];
	}
}
