using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

	public float speed = 10f;
	public float fireRate = 0.3f;
	public float health = 10;
	public int score = 100; //points earned for destroying this

	public int showDamageForFrames = 2;
	public float powerUpDropChance = 1f; //chacne to drop powerup

	public bool _______________;

	public Color[] originalColors;
	public Material[] materials;
	public int remainingDamageFrames = 0;
	public Bounds bounds; //Bounds of this and its children
	public Vector3 boundsCenterOffset; //Dist of bounds.center from position

	// Use this for initialization
	void Awake () {
		materials = Utils.GetAllMaterials (gameObject);
		originalColors = new Color[materials.Length];
		for (int i=0; i < materials.Length; i++) {
			originalColors[i] = materials[i].color;
		}
		InvokeRepeating ("CheckOffScreen", 0f, 2f);
	}
	
	// Update is called once per frame
	void Update () {
		Move ();
		if (remainingDamageFrames>0) {
			remainingDamageFrames--;
			if(remainingDamageFrames == 0) {
				UnShowDamage();
			}
		}
	}

	public virtual void Move() {
		Vector3 tempPos = pos;
		tempPos.y -= speed * Time.deltaTime;
		pos = tempPos;
	}

	//this is a property: a method that acts like a field
	public Vector3 pos {
		get {
			return(this.transform.position);
		}
		set {
			this.transform.position = value;
		}
	}

	void CheckOffScreen () {
		//if bounds are still their default value
		if (bounds.size == Vector3.zero) {
			//then set them
			bounds = Utils.CombineBoundsOfChildren (this.gameObject);
			//also find the diff between bounds.center & transform.position
			boundsCenterOffset = bounds.center - transform.position;
		}

		//every time, ypdate the bounds to the current posiiton
		bounds.center = transform.position + boundsCenterOffset;
		//check to see whether the bounds are completely offscreen
		Vector3 off = Utils.ScreenBoundsCheck (bounds, BoundsTest.offScreen);
		if (off != Vector3.zero) {
			//if this enemy has gone off bottom edge of screen
			if (off.y < 0) {
				Destroy (this.gameObject);
			}
		}
	}

	void OnCollisionEnter (Collision coll) {
		GameObject other = coll.gameObject;
		switch (other.tag) {
		case "ProjectileHero":
			Projectile p = other.GetComponent<Projectile>();
			//Enemies don't take damage unless they're conscreen
			//this stops player from shooting them before theyre visible
			bounds.center = transform.position + boundsCenterOffset;
			if (bounds.extents == Vector3.zero || Utils.ScreenBoundsCheck (bounds, BoundsTest.offScreen) != Vector3.zero) {
				Destroy(other);
				break;
			}

			//Hurt this enemy
			ShowDamage();
			//Get dmg from Projectile.type & Main.W_DEFS
			health -= Main.W_DEFS[p.type].damageOnHit;
			if (health <= 0) {
				Main.S.ShipDestroyed(this);
				Destroy(this.gameObject);
			}
			Destroy(other);
			break;
		case "ProjectileHeroLaser":
			Projectile l = other.GetComponent<Projectile>();
			//Enemies don't take damage unless they're conscreen
			//this stops player from shooting them before theyre visible
			bounds.center = transform.position + boundsCenterOffset;
			if (bounds.extents == Vector3.zero || Utils.ScreenBoundsCheck (bounds, BoundsTest.offScreen) != Vector3.zero) {
				Destroy(other);
				break;
			}
			ShowDamage();
			//Get dmg from Projectile.type & Main.W_DEFS
			health -= Main.W_DEFS[l.type].continuousDamage;
			if (health <= 0) {
				Main.S.ShipDestroyed(this);
				Destroy(this.gameObject);
			}
			//Destroy(other);
			break;
		}
	}

	void ShowDamage() {
		foreach (Material m in materials) {
			m.color = Color.red;
		}
		remainingDamageFrames = showDamageForFrames;
	}

	void UnShowDamage() {
		for (int i=0; i < materials.Length; i++ ) {
			materials[i].color = originalColors[i];
		}
	}
}
