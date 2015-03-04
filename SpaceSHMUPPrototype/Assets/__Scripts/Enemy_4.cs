using UnityEngine;
using System.Collections;

[System.Serializable]
public class Part {
	//these three fields need to be defined in inspector
	public string name;
	public float health;
	public string[] protectedBy;

	//these two fields are automatically in STart()
	public GameObject go;
	public Material mat;
}

public class Enemy_4 : Enemy {
	//Enemy_4 willl choose a rand point on screen, move, then choose another point and move until it dies

	public Vector3[] points;
	public float timeStart;
	public float duration = 4; //duration of movements
	public Part[] parts; //array of ship parts

	// Use this for initialization
	void Start () {
		points = new Vector3[2];
		points[0] = pos;
		points[1] = pos;

		InitMovement();

		//Cache GameObject & Material of each PArt in parts
		Transform t;
		foreach(Part prt in parts) {
			t = transform.Find (prt.name);
			if (t != null) {
				prt.go = t.gameObject;
				prt.mat = prt.go.GetComponent<Renderer>().material;
			}
		}
	}
	
	// Update is called once per frame
	void InitMovement () {
	//Pick a new point to move to that is on screen

		Vector3 p1 = Vector3.zero;
		float esp = Main.S.enemySpawnPadding;
		Bounds cBounds = Utils.camBounds;
		p1.x = Random.Range (cBounds.min.x + esp, cBounds.max.x - esp);
		p1.y = Random.Range (cBounds.min.y + esp, cBounds.max.y - esp);

		points[0] = points[1];
		points[1] = p1;

		//reset the time
		timeStart = Time.time;
	}

	public override void Move () {
		//completely ovverrides Enemy.Move() with a linear interpolation
		float u = (Time.time-timeStart)/duration;
		if(u>=1) { //if u >= 1...
			InitMovement();
			u=0;
		}

		u = 1 - Mathf.Pow (1-u, 2); //Apply eas out easing to u

		pos = (1-u)*points[0] + u*points[1];
	}

	//This will override the OnCollisionEnter that is part of Enemy.cs
	//Because of the way that MonoBehavior declares common Unity functions
	// like OnCollisionEnter(), the override keyword is not necessary
	void OnCollisionEnter (Collision coll) {
		GameObject other = coll.gameObject;
		switch(other.tag) {
		case "ProjectileHero":
			Projectile p = other.GetComponent<Projectile>();
			//Enemies don't take damage unless they're on screen
			bounds.center = transform.position + boundsCenterOffset;
			if (bounds.extents == Vector3.zero || Utils.ScreenBoundsCheck ( bounds, BoundsTest.offScreen) != Vector3.zero) {
				Destroy(other);
				break;
			}

			GameObject goHit = coll.contacts[0].thisCollider.gameObject;
			Part prtHit = FindPart(goHit);
			if (prtHit == null) { // if prtHit wasnt' found
				goHit = coll.contacts[0].otherCollider.gameObject;
				prtHit = FindPart(goHit);
			}
			if (prtHit.protectedBy != null) {
				foreach (string s in prtHit.protectedBy) {
					if (!Destroyed(s)) {
						//then dont dmg part yet
						Destroy(other);
						return;
					}
				}
			}
			//its not protected so make it take dmg
			//get dmg amt from projectile.type & Main.W_DEFS
				prtHit.health -= Main.W_DEFS[p.type].damageOnHit;
			ShowLocalizedDamage(prtHit.mat);
			if (prtHit.health <= 0) {
				//instead of Destroying this enemy, disable dmgd part
				prtHit.go.SetActive (false);
			}
			//check to see if whole ship is destroyed
			bool allDestroyed = true;
			foreach (Part prt in parts) {
				if (!Destroyed(prt)) { //if a part still exists
					allDestroyed = false;
					break;
				}
			}
			if (allDestroyed) {
				Main.S.ShipDestroyed (this);
				Destroy(this.gameObject);
			}
			Destroy(other);
			break;
		case "ProjectileHeroLaser":
			Projectile x = other.GetComponent<Projectile>();
			//Enemies don't take damage unless they're on screen
			bounds.center = transform.position + boundsCenterOffset;
			if (bounds.extents == Vector3.zero || Utils.ScreenBoundsCheck ( bounds, BoundsTest.offScreen) != Vector3.zero) {
				Destroy(other);
				break;
			}
			
			GameObject goHitx = coll.contacts[0].thisCollider.gameObject;
			Part prtHitx = FindPart(goHitx);
			if (prtHitx == null) { // if prtHit wasnt' found
				goHitx = coll.contacts[0].otherCollider.gameObject;
				prtHitx = FindPart(goHitx);
			}
			if (prtHitx.protectedBy != null) {
				foreach (string s in prtHitx.protectedBy) {
					if (!Destroyed(s)) {
						//then dont dmg part yet
						//Destroy(other);
						return;
					}
				}
			}
			//its not protected so make it take dmg
			//get dmg amt from projectile.type & Main.W_DEFS
			prtHitx.health -= Main.W_DEFS[x.type].continuousDamage;
			ShowLocalizedDamage(prtHitx.mat);
			if (prtHitx.health <= 0) {
				//instead of Destroying this enemy, disable dmgd part
				prtHitx.go.SetActive (false);
			}
			//check to see if whole ship is destroyed
			bool allDestroyedx = true;
			foreach (Part prt in parts) {
				if (!Destroyed(prt)) { //if a part still exists
					allDestroyedx = false;
					break;
				}
			}
			if (allDestroyedx) {
				Main.S.ShipDestroyed (this);
				Destroy(this.gameObject);
			}
			//Destroy(other);
			break;
		}
	}

	Part FindPart (string n) {
		foreach (Part prt in parts) {
			if (prt.name == n) {
				return (prt);
			}
		}
		return (null);
	}
	Part FindPart (GameObject go) {
		foreach (Part prt in parts) {
			if (prt.go == go) {
				return (prt);
			}
		}
		return(null);
	}

	bool Destroyed (GameObject go) {
		return (Destroyed (FindPart(go)));
	}
	bool Destroyed (string n) {
		return (Destroyed (FindPart (n)));
	}
	bool Destroyed (Part prt) {
		if (prt == null) { //if no real part was passed in
			return (true); //return true meaning yes it was destroyed
		}
		//Resturns te result of the comparison : prt.health <= 0
		//if prt.health is 0 or less, returns true
		return (prt.health<=0);
	}

	void ShowLocalizedDamage (Material m) {
		m.color = Color.red;
		remainingDamageFrames = showDamageForFrames;
	}
}
