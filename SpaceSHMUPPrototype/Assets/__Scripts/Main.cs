using UnityEngine;
using System.Collections;
using System.Collections.Generic; //required to use Lists or Dictionaries

public class Main : MonoBehaviour {
	static public Main S;
	static public Dictionary<WeaponType, WeaponDefinition> W_DEFS;


	public GameObject[] prefabEnemies;
	public float enemySpawnPerSecond = 0.5f;// # enemies/second
	public float enemySpawnPadding = 1.5f; //Padding for position
	public WeaponDefinition[] weaponDefinitions;
	public GameObject prefabPowerUp;
	public WeaponType[] powerUpFrequency = new WeaponType[] {
		WeaponType.blaster, WeaponType.laser, 
		WeaponType.phaser,
		WeaponType.spread,
		WeaponType.shield
	};

	public bool _____________;

	public WeaponType[] activeWeaponTypes;
	public float enemySpawnRate; //delay between Enemy Spawns


	// Use this for initialization
	void Awake () {
		S = this;
		//set utils.camBounds
		Utils.SetCameraBounds (this.GetComponent<Camera>());
		//0.5 enemies/secdon = enemySpawnRate of 2
		enemySpawnRate = 1f/enemySpawnPerSecond;
		//Invoke call SpawnEney() once after a 2 second delay
		Invoke ("SpawnEnemy", enemySpawnRate);

		//a generic dictionary with WeaponType as the key
		W_DEFS = new Dictionary<WeaponType, WeaponDefinition>();
			foreach (WeaponDefinition def in weaponDefinitions) {
			W_DEFS[def.type] = def;
			}
		}

	static public WeaponDefinition GetWeaponDefinition (WeaponType wt) {
		//check to make sure that htey key exists in the Dictionary
		//attempting to retrieve the key that didnt exist would throw an error
		//so the following statement is important
		if (W_DEFS.ContainsKey (wt)) {
			return(W_DEFS[wt]);
		}
		//this will return a def for WeaponType.none
		return (new WeaponDefinition());
	}

	void Start() {
		activeWeaponTypes = new WeaponType[weaponDefinitions.Length];
		for (int i=0; i<weaponDefinitions.Length; i++) {
			activeWeaponTypes[i] = weaponDefinitions[i].type;
		}
	}

	public void SpawnEnemy() {
		//pick a random enemy prefab to instantiate
		int ndx = Random.Range (0, prefabEnemies.Length);
		GameObject go = Instantiate (prefabEnemies[ndx]) as GameObject;
		//position the neemy above the screen with a random x pos
		Vector3 pos = Vector3.zero;
		float xMin = Utils.camBounds.min.x + enemySpawnPadding;
		float xMax = Utils.camBounds.max.x - enemySpawnPadding;
		pos.x = Random.Range (xMin, xMax);
		pos.y = Utils.camBounds.max.y + enemySpawnPadding;
		go.transform.position = pos;
		//call SpawnEnemy() again in a couple seconds
		Invoke ("SpawnEnemy", enemySpawnRate);


	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void DelayedRestart (float delay) {
		//invoke Restart() in delay seconds
		Invoke ("Restart", delay);
	}

	public void Restart() {
		Application.LoadLevel ("_Scene_0");
	}

	public void ShipDestroyed (Enemy e) {
		if (Random.value <= e.powerUpDropChance) {
		int ndx = Random.Range (0,powerUpFrequency.Length);
			WeaponType puType = powerUpFrequency[ndx];

			//spawn powerup
			GameObject go = Instantiate (prefabPowerUp) as GameObject;
			PowerUp pu = go.GetComponent<PowerUp>();
			pu.SetType (puType);

			pu.transform.position = e.transform.position;
		}
	}

}
