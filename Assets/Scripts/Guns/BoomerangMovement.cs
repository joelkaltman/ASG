using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BoomerangMovement : NetworkBehaviour {

	public int damage;
	public float speed;
	public float rotationSpeed;
	public int maxBounces;
	public int distanceBounce;
	public bool comesBack;
	public bool addCountOnReturn;

	GameObject target;
	[HideInInspector] public GameObject player;

	bool found;
	bool searched;
	int bounces;
	int lastBouncedEnemyID;

	// Use this for initialization
	void Start () 
	{
		if (!IsHost)
		{
			enabled = false;
			return;
		}
		
		target = null;
		found = false;
		searched = false;
		bounces = 0;
		lastBouncedEnemyID = -1;

		transform.rotation = Quaternion.Euler (180, 0, -90);

		SearchTarget();
	}

	// Update is called once per frame
	void Update () 
	{
		if (target != null) {
			Vector3 dir = target.transform.position - transform.position;
			dir.Normalize ();
			dir.y = 0;
			transform.position += speed * Time.deltaTime * dir;
			transform.Rotate (rotationSpeed, 0, 0);
		}else{
			//Destroy (gameObject);
		}
	}

	void SearchTarget()
	{
		target = null;
		found = false;
		if (bounces < maxBounces) {
			List<GameObject> enemies = EnemiesManager.Instance.EnemiesInstances;

			float closest = 99999;
			for (int i = 0; i < enemies.Count; i++) {
				float distance = Vector3.Distance (enemies [i].transform.position, transform.position);
				EnemyStats stats = enemies[i].GetComponent<EnemyStats> ();
				if (stats.life > 0 && distance < distanceBounce && distance < closest && !(enemies[i].GetInstanceID() == lastBouncedEnemyID)) {
					target = enemies [i];
					found = true;
					closest = distance;
				}
			}
		}
		if (bounces >= maxBounces || found == false){
			if (comesBack) {
				target = player;
			} else {
				gameObject.layer = 12;
				GetComponent<Rigidbody> ().useGravity = true;
				GetComponent<Collider> ().isTrigger = false;
				Destroy (gameObject, 2);
			}
		}
		searched = true;
	}

	void OnTriggerEnter(Collider col)
	{
		if (col.gameObject.tag == "Enemy" && col.gameObject.GetInstanceID() != lastBouncedEnemyID) {
			EnemyStats stats = col.gameObject.GetComponent<EnemyStats> ();
			stats.RecieveDamage (damage, true, true);
			GetComponent<AudioSource> ().Play ();
			lastBouncedEnemyID = col.gameObject.GetInstanceID();
			bounces++;
			SearchTarget ();
		} else if(col.gameObject.tag == "Player" && !found && searched) {
			Destroy (gameObject);
		}
	}

	void OnTriggerStay(Collider col){
		OnTriggerEnter (col);
	}

	void OnDestroy(){
		if (comesBack) {
			PlayerGuns playerguns = player.GetComponent<PlayerGuns> ();
			playerguns.BoomerangReturned (gameObject.name, addCountOnReturn);
		}
	}
}
