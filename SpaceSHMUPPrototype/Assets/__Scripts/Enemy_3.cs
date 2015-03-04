using UnityEngine;
using System.Collections;

public class Enemy_3 : Enemy {

	//moves in a bezier curve which is lineare interpolation between more than two points

	public Vector3[] points;
	public float birthTime;
	public float lifeTime = 10;

	// Use this for initialization
	void Start () { //start works well because it is not used by class Enemy
		points = new Vector3[3];

		points[0] = pos;
		
		float xMin = Utils.camBounds.min.x+Main.S.enemySpawnPadding;
		float xMax = Utils.camBounds.max.x-Main.S.enemySpawnPadding;

		Vector3 v;
		//pick a random middle position in the bot half of screen
		v = Vector3.zero;
		v.x = Random.Range (xMin, xMax);
		v.y = Random.Range (Utils.camBounds.min.y, 0);
		points[1] = v;

		//pick final random pos above top of screen
		v = Vector3.zero;
		v.y = pos.y;
		v.x = Random.Range (xMin, xMax);
		points[2] = v;

		birthTime = Time.time;
	}

	public override void Move(){
		//bezier curves based on a u value between 0 & 1
		float u = (Time.time - birthTime) / lifeTime;

		if (u > 1) {
			//this enemy 3 has finished its life
			Destroy (this.gameObject);
			return;
		}

		//interpolate the three bezier curve points
		Vector3 p01, p12;
		u=u-0.2f*Mathf.Sin (u*Mathf.PI*2);
		p01 = (1-u)*points[0] + u*points[1];
		p12 = (1-u)*points[1] + u*points[2];
		pos = (1-u)*p01 + u*p12;
	}
}
