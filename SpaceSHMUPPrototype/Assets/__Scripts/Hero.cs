using UnityEngine;
using System.Collections;

public class Hero : MonoBehaviour {

	static public Hero S; // Singleton

	public float gameRestartDelay = 2f;
	
	//ship movement
	public float speed = 30;
	public float rollMult = -45;
	public float pitchMult = 30;

	public GameObject powerUpSound;

	[SerializeField]
	private float _shieldLevel = 1;

	//Weapon fields
	public Weapon[] weapons;

	public bool _____________;

	public Bounds bounds;

	//Declare a new delegate type WeaponFireDelegate
	public delegate void WeaponFireDelegate();
	//create a WeaponFireDelegate field named fireDelegate
	public WeaponFireDelegate fireDelegate;

	void Awake() {
		S = this; //set the singleton
		bounds = Utils.CombineBoundsOfChildren (this.gameObject);
	}

	// Use this for initialization
	void Start () {
		
		//reset the wepaons to start _hero with 1 blaster
		ClearWeapons();
		weapons[0].SetType(WeaponType.blaster);
	}
	
	// Update is called once per frame
	void Update () {
		//pull in info from the input class
		float xAxis = Input.GetAxis ("Horizontal");
		float yAxis = Input.GetAxis ("Vertical");

		//change transform.pos based on the axes
		Vector3 pos = transform.position;
		pos.x += xAxis * speed * Time.deltaTime;
		pos.y += yAxis * speed * Time.deltaTime;
		transform.position = pos;

		bounds.center = transform.position;
		//keep this ship constrained to the screen bounds
		Vector3 off = Utils.ScreenBoundsCheck (bounds, BoundsTest.onScreen);
		if (off != Vector3.zero) {
			pos -= off;
			transform.position = pos;
		}

		//rotate ship to make it feel more dynamic
		transform.rotation = Quaternion.Euler(yAxis * pitchMult, xAxis * rollMult,0);

		//Use the fireDelegate to fire Weapons
		//First, make sure the Axis("Jump") button is pressed
		//then ensure that fireDelegate isn't null to avoid an error
		if (Input.GetAxis ("Jump") == 1 && fireDelegate != null) {
			fireDelegate();
		}

	}

	public GameObject lastTriggerGo = null;

	void OnTriggerEnter (Collider other) {

		GameObject go = Utils.FindTaggedParent (other.gameObject);
		if (go != null) {
			print ("Triggered: "+go.name);
			//make sure its not the same triggering go as last time
			if (go == lastTriggerGo) {
				return;
			}
			lastTriggerGo = go;

			if (go.tag == "Enemy") {
				//if shield was triggered by enemy decrease level of shile d by 1
				shieldLevel--;
				Destroy(go);
			} else if (go.tag == "PowerUp") {
				AbsorbPowerUp(go);
			}
		} else {
			//otherwise annoucen original other.gameObhect
			print ("Triggered: "+other.gameObject.name);
		}
	}

	public float shieldLevel {
		get {
			return (_shieldLevel);
		}
		set {
			_shieldLevel = Mathf.Min (value, 4);
			//if shield is going to be set to less than zero
			if (value < 0) {
				Destroy(this.gameObject);
				Main.S.DelayedRestart (gameRestartDelay); //tell Main.S to restart game after delay
			}
		}
	}
	public void AbsorbPowerUp (GameObject go) {
		GameObject powerUpSoundInstance = Instantiate (powerUpSound) as GameObject;
		PowerUp pu = go.GetComponent<PowerUp>();
		switch (pu.type) {
		case WeaponType.shield:
			shieldLevel++;
			break;

		default:
			if (pu.type == weapons[0].type) {//increase number of weapons
				Weapon w = GetEmptyWeaponSlot(); //find availabel weapon
				if(w != null) {
					w.SetType (pu.type);
					Weapon q = GetFullWeaponSlot();
					q.SetType (pu.type);

					weapons[0].SetType (pu.type);
				}
			} else {
				//if this is a diff weapon
				Weapon q = GetFullWeaponSlot();
				foreach (Weapon x in weapons) {
					if(x.type != WeaponType.none){
						x.SetType (pu.type);
					}
				}
				//foreach (Weapon x in weapons) {
				//	x.SetType(pu.type);

				//}
				
				weapons[0].SetType (pu.type);
			}
			break;
		}
		pu.AbsorbedBy (this.gameObject);
	}

	Weapon GetEmptyWeaponSlot() {
			for (int i=0; i<weapons.Length; i++) {
			if(weapons[i].type == WeaponType.none) {
				return (weapons[i]);
			}
		}
		return(null);
	}

	Weapon GetFullWeaponSlot() {
		for (int i=0; i<weapons.Length; i++) {
			if(weapons[i].type != WeaponType.none) {
				return (weapons[i]);
			}
		}
		return(null);
	}

	void ClearWeapons() {
		foreach (Weapon w in weapons) {
			w.SetType(WeaponType.none);
		}
	}
}