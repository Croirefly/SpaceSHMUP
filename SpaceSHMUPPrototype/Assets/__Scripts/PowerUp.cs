using UnityEngine;
using System.Collections;

public class PowerUp : MonoBehaviour {
	//this is an unusual but handy use of Vector2s. x holds a min value
	// and y a max value for a Random.Range() Attribute that will be called later
	public Vector2 rotMinMax = new Vector2(15,90);
	public Vector2 driftMinMax = new Vector2(.25f,2);
	public float lifeTime = 6f; //seconds the powerpup exists
	public float fadeTime = 4f; //seconds it will then fade

	public bool ______________;

	public WeaponType type;
	public GameObject cube;
	public TextMesh letter;
	public Vector3 rotPerSecond;
	public float birthTime;

	void Awake() {
		cube = transform.Find("Cube").gameObject;
		letter = GetComponent<TextMesh>();

		Vector3 vel = Random.onUnitSphere; //get rnadom xyz velcoity
		//Random.onUnitSphere gives you a vecto point that is somewhere on surphace of sphere with radius 1m
		vel.z = 0;
		vel.Normalize (); //make the length of the vel 1
		//Normalizing a vector 3 makes it length 1m
		vel *= Random.Range (driftMinMax.x,driftMinMax.y);
		GetComponent<Rigidbody>().velocity = vel;

		transform.rotation = Quaternion.identity; //Quaternion.identity is equal to no rotation

		rotPerSecond = new Vector3 (Random.Range (rotMinMax.x,rotMinMax.y),Random.Range (rotMinMax.x,rotMinMax.y),Random.Range (rotMinMax.x,rotMinMax.y));

		InvokeRepeating ("CheckOffScreen", 2f, 2f);

		birthTime = Time.time;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		cube.transform.rotation = Quaternion.Euler (rotPerSecond*Time.time);

		float u = (Time.time - (birthTime+lifeTime)) / fadeTime;

		if (u >= 1) {
			Destroy (this.gameObject);
			return;
		}

		if (u>0) {
			Color c = cube.GetComponent<Renderer>().material.color;
			c.a = 1f-u;
			cube.GetComponent<Renderer>().material.color = c;
			//fade the letter
			c = letter.color;
			c.a = 1f - (u*0.5f);
			letter.color = c;
		}
	}

	public void SetType (WeaponType wt) {
		WeaponDefinition def = Main.GetWeaponDefinition (wt);
		cube.GetComponent<Renderer>().material.color = def.color;

		letter.text = def.letter;
		type = wt;
	}

	public void AbsorbedBy (GameObject target) {
		//called when powrup by hero class when it is collecteed
		Destroy(this.gameObject);
	}

	void CheckOffScreen() {
		if (Utils.ScreenBoundsCheck (cube.GetComponent<Collider>().bounds, BoundsTest.offScreen) != Vector3.zero) {
			Destroy (this.gameObject);
		}
	}
}
