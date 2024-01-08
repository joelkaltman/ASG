using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoomerangMovement : MonoBehaviour {

	public int damage;
	public float speed;
	public float rotationSpeed;
	public int maxBounces;
	public int distanceBounce;
	public bool comesBack;
	public bool addCountOnReturn;

	GameObject target;
	GameObject player;

	Vector3 initialForward;
	bool found;
	bool searched;
	int bounces;
	int lastBouncedEnemyID;

	// Use this for initialization
	void Start () {
		target = null;
		initialForward = this.transform.forward;
		found = false;
		searched = false;
		bounces = 0;
		lastBouncedEnemyID = -1;
		player = PlayerStats.Instance.getPlayer();

		this.transform.rotation = Quaternion.Euler (180, 0, -90);

		searchTarget ();
	}

	// Update is called once per frame
	void Update () {
		if (target != null) {
			Vector3 dir = target.transform.position - this.transform.position;
			dir.Normalize ();
			dir.y = 0;
			this.transform.position += speed * Time.deltaTime * dir;
			this.transform.Rotate (rotationSpeed, 0, 0);
		}else{
			//Destroy (this.gameObject);
		}
	}

	void searchTarget(){
		target = null;
		found = false;
		if (bounces < maxBounces) {
			List<GameObject> enemies = EnemiesManager.Instance.getInstantiatedEnemies ();

			float closest = 99999;
			for (int i = 0; i < enemies.Count; i++) {
				float distance = Vector3.Distance (enemies [i].transform.position, this.transform.position);
				EnemyStats stats = enemies[i].GetComponent<EnemyStats> ();
				if (stats.life > 0 && distance < distanceBounce && distance < closest && !(enemies[i].GetInstanceID() == lastBouncedEnemyID)) {
					this.target = enemies [i];
					found = true;
					closest = distance;
				}
			}
		}
		if (bounces >= maxBounces || found == false){
			if (comesBack) {
				target = player;
			} else {
				this.gameObject.layer = 12;
				this.GetComponent<Rigidbody> ().useGravity = true;
				this.GetComponent<Collider> ().isTrigger = false;
				Destroy (this.gameObject, 2);
			}
		}
		searched = true;
	}

	void OnTriggerEnter(Collider col)
	{
		if (col.gameObject.tag == "Enemy" && col.gameObject.GetInstanceID() != lastBouncedEnemyID) {
			EnemyStats stats = col.gameObject.GetComponent<EnemyStats> ();
			stats.RecieveDamage (damage, true, true);
			this.GetComponent<AudioSource> ().Play ();
			lastBouncedEnemyID = col.gameObject.GetInstanceID();
			bounces++;
			this.searchTarget ();
		} else if(col.gameObject.tag == "Player" && !found && searched) {
			Destroy (this.gameObject);
		}
	}

	void OnTriggerStay(Collider col){
		this.OnTriggerEnter (col);
	}

	void OnDestroy(){
		if (comesBack) {
			PlayerGuns playerguns = player.GetComponent<PlayerGuns> ();
			playerguns.BoomerangReturned (this.gameObject.name, addCountOnReturn);
		}
	}
}
