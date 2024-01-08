using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokeballCollision : MonoBehaviour {

	public float distanceCatch;
	public int chanceCatch;
	public GameObject explosionParticles;
	public AudioClip soundCatch;
	public AudioClip soundOut;

	GameObject explosionInstance;

	GameObject capturedEnemy;

	AudioSource audio;

	void OnCollisionEnter(Collision col)
	{
		audio = this.GetComponent<AudioSource> ();

		CatchEnemy ();


		this.GetComponent<Collider> ().enabled = false;
		this.GetComponent<Rigidbody> ().constraints = RigidbodyConstraints.FreezeAll;
		this.transform.position = new Vector3 (this.transform.position.x, 5.1f, this.transform.position.z);
	}

	void CatchEnemy()
	{
		List<GameObject> enemies = EnemiesManager.Instance.getInstantiatedEnemies();

		for (int i = 0; i < enemies.Count; i++) {
			float distance = Vector3.Distance (enemies [i].transform.position, this.transform.position);
			if (distance < distanceCatch) {
				this.capturedEnemy = enemies [i];
				this.capturedEnemy.SetActive(false);

				audio.clip = soundCatch;
				audio.Play ();

				explosionInstance = Instantiate (explosionParticles, this.transform.position, Quaternion.identity);
				Destroy (explosionInstance, 1);

				break;
			}
		}

		if (capturedEnemy == null) {
			audio.clip = soundOut;
			audio.Play ();
		}

		Invoke ("DecideCatch", 4f);
	}

	void DecideCatch(){
		if (capturedEnemy == null) {
			Destroy (this.gameObject);
			return;
		}

		int rnd = Random.Range (0, 100);
		if (rnd < chanceCatch) {
			// captured
			EnemyStats stats = this.capturedEnemy.GetComponent<EnemyStats> ();
			if (stats != null) {
				stats.RecieveDamage (99999, false, true);
			}
		} else {
			this.capturedEnemy.SetActive(true);
			explosionInstance = Instantiate (explosionParticles, this.transform.position, Quaternion.identity);
			Destroy (explosionInstance, 1);

			audio.clip = soundOut;
			audio.Play ();
		}


		Destroy (this.gameObject);
	}
}
