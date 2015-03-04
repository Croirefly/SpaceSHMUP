using UnityEngine;
using System.Collections;

//this is an enum of the various possible weapon types
//it also includes a "shield" type to allow a shield power-up
//items marked [ni] abelow are not implemnted in this book
public enum WeaponType {
	none, //the default / no weapon
	blaster, //a simple blaster
	spread, // two shots simulatenously
	phaser, //shots that move in waves [ni]
	missile, //homing missiles [ni]
	laser, //dmg over time [ni]
	shield // raise shieldLevel
}

//the WeaponDefinition class allows you to set the properties of a sepcific weapon in the Inspector
//Main has an array of WeaponDefinitions that makes this possible.
//[System.Serializable] tells Unity to try to view WeaponDefinition
//int he Inspector pane. It doesn't work for everything, but it
//will work for simple classes like this!
[System.Serializable]
public class WeaponDefinition {
	public WeaponType type = WeaponType.none;
	public string letter; //the letter to show on the power-up
	public Color color = Color.white; // color of Collar & power-up
	public GameObject projectilePrefab; //prefab for projectiles
	public Color projectileColor = Color.white;
	public float damageOnHit = 0; //amnt of dmg caused
	public float continuousDamage = 0; //dmg per second (laser)
	public float delayBetweenShots = 0;
	public float velocity = 20; //speed of projectiles
} //these are set in class Main.

public class Weapon : MonoBehaviour {

	static public Transform PROJECTILE_ANCHOR;
	public GameObject blasterSound;
	public GameObject laserSound;
	public GameObject phaserSound;
	public bool playSound;
	public bool ___________________;
	[SerializeField]
	private WeaponType _type = WeaponType.blaster;
	public WeaponDefinition def;
	public GameObject collar;
	public GameObject laserContainer;
	public float lastShot; //time last shot was fired

	void Awake() {
		collar = transform.Find ("Collar").gameObject;
		laserContainer = GameObject.Find ("LaserContainer");
	}

	// Use this for initialization
	void Start () {
		//Call SetType() properly for the default _type
		SetType (_type);

		if (PROJECTILE_ANCHOR == null) {
			GameObject go = new GameObject ("_Projectile_Anchor");
			PROJECTILE_ANCHOR = go.transform;
		}
		//find the fireDelegate of the parent
		GameObject parentGO = transform.parent.gameObject;
		if (parentGO.tag == "Hero") {
			Hero.S.fireDelegate += Fire;
		}
	}

	public WeaponType type {
		get { return(_type); }
		set { SetType (value); }
	}

	public void SetType (WeaponType wt) {
		_type = wt;
		if (type == WeaponType.none) {
			this.gameObject.SetActive (false);
			return;
		}else {
			this.gameObject.SetActive (true);
		}
		def = Main.GetWeaponDefinition(_type);
		collar.GetComponent<Renderer>().material.color = def.color;
		lastShot = 0; //you can always fire immediately after _type is set
	}

	public void Fire() {
		//if this.gameObject is inactive return
		if (!gameObject.activeInHierarchy) return;
		//If it hasn't been enough time between shots, return
		if (Time.time - lastShot < def.delayBetweenShots) {
			return;
		}

		Projectile p;
		switch (type) {
		case WeaponType.blaster:
			p = MakeProjectile();
			p.GetComponent<Rigidbody>().velocity = Vector3.up * def.velocity;
			if(playSound){
				GameObject blasterSoundInstance = Instantiate (blasterSound) as GameObject;
			}
			break;

		case WeaponType.spread:
			p = MakeProjectile();
			p.GetComponent<Rigidbody>().velocity = Vector3.up * def.velocity;
			p = MakeProjectile();
			p.GetComponent<Rigidbody>().velocity = new Vector3 (-.2f, 0.9f, 0) * def.velocity;
			p = MakeProjectile();
			p.GetComponent<Rigidbody>().velocity = new Vector3 (.2f, 0.9f, 0) * def.velocity;
			if(playSound){
				GameObject spreadSoundInstance = Instantiate (blasterSound) as GameObject;
			}
			break;
		case WeaponType.laser:
			p = MakeLaser ();
			p.GetComponent<Rigidbody>().velocity = Vector3.up * def.velocity;
			if(playSound){
				GameObject laserSoundInstance = Instantiate (laserSound) as GameObject;
			}

			break;
		case WeaponType.phaser:
			p = MakeProjectile ();
			p.GetComponent<Rigidbody>().velocity = Vector3.up * def.velocity;
			if(playSound){
				GameObject phaserSoundInstance = Instantiate (phaserSound) as GameObject;
			}
			break;
		}
	}

	public Projectile MakeProjectile() {
		GameObject go = Instantiate (def.projectilePrefab) as GameObject;
		if (transform.parent.gameObject.tag == "Hero") {
			go.tag = "ProjectileHero";
			go.layer = LayerMask.NameToLayer ("ProjectileHero");
		}else {
			go.tag = "Projectileenemy";
			go.layer = LayerMask.NameToLayer ("ProjectileEnemy");
		}
		go.transform.position = collar.transform.position + new Vector3 (0,1.3f,0);
		go.transform.parent = PROJECTILE_ANCHOR;
		Projectile p = go.GetComponent<Projectile>();
		p.type = type;
		lastShot = Time.time;
		return (p);
	}
	public Projectile MakeLaser() {
		GameObject go = Instantiate (def.projectilePrefab) as GameObject;
		if (transform.parent.gameObject.tag == "Hero") {
			go.tag = "ProjectileHeroLaser";
			go.layer = LayerMask.NameToLayer ("ProjectileHeroLaser");
		}else {
			go.tag = "Projectileenemy";
			go.layer = LayerMask.NameToLayer ("ProjectileEnemy");
		}
		go.transform.position = collar.transform.position + new Vector3(0,52,0);
		go.transform.parent = laserContainer.transform;
		Projectile p = go.GetComponent<Projectile>();
		p.type = type;
		lastShot = Time.time;
		return (p);
	}


	
	// Update is called once per frame
	void Update () {
	
	}
}
