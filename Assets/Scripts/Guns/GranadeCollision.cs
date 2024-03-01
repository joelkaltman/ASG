using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GranadeCollision : PlayerOwned {

	public int damage;
	public float distanceDamage;
	public GameObject explosionParticles;

	GameObject explosionInstance;
	
	void OnTriggerEnter(Collider col)
	{
		if (col.gameObject.GetComponent<PlayerMovement>() != null)
			return;
		
		this.GetComponent<AudioSource>().Play ();

		DamageEnemies ();

		explosionInstance = Instantiate (explosionParticles, this.transform.position, Quaternion.identity);
		explosionInstance.GetComponent<NetworkObject>()?.Spawn(true);

		this.GetComponent<Collider> ().enabled = false;
		this.GetComponent<Renderer> ().enabled = false;
		
		StartCoroutine(DestroyExplosion());
	}

	IEnumerator DestroyExplosion()
	{
		yield return new WaitForSeconds(3);
		explosionInstance.GetComponent<NetworkObject>()?.Despawn();
		gameObject.GetComponent<NetworkObject>()?.Despawn();
	}

	void DamageEnemies()
	{
		List<GameObject> enemies = EnemiesManager.Instance.EnemiesInstances;

		for (int i = 0; i < enemies.Count; i++) {
			float distance = Vector3.Distance (enemies [i].transform.position, this.transform.position);
			if (distance < distanceDamage) {
				EnemyStats stats = enemies [i].GetComponent<EnemyStats> ();
				if (stats != null) {
					stats.RecieveDamage (player, damage, true, true);
				}
			}
		}
	}
}
