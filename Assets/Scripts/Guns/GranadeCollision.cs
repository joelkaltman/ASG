using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GranadeCollision : MonoBehaviour {

	public int damage;
	public float distanceDamage;
	public GameObject explosionParticles;

	GameObject explosionInstance;

	void OnCollisionEnter(Collision col)
	{
		this.GetComponent<AudioSource>().Play ();

		DamageEnemies ();

		explosionInstance = Instantiate (explosionParticles, this.transform.position, Quaternion.identity);
		Invoke ("DestroyExplosion", 3);

		this.GetComponent<Collider> ().enabled = false;
		this.GetComponent<Renderer> ().enabled = false;
	}

	void DestroyExplosion()
	{
		Destroy (explosionInstance);
		Destroy (this.gameObject);
	}

	void DamageEnemies()
	{
		List<GameObject> enemies = EnemiesManager.Instance.getInstantiatedEnemies();

		for (int i = 0; i < enemies.Count; i++) {
			float distance = Vector3.Distance (enemies [i].transform.position, this.transform.position);
			if (distance < distanceDamage) {
				EnemyStats stats = enemies [i].GetComponent<EnemyStats> ();
				if (stats != null) {
					stats.RecieveDamage (damage, true, true);
				}
			}
		}
	}
}
