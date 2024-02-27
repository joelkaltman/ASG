using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCollisionVoodoo : MonoBehaviour {

	public int damage;
	public GameObject explosionParticles;

	GameObject explosionInstance;

	private void Start()
	{
		if (!MultiplayerManager.Instance.IsHostReady)
		{
			enabled = false;
			return;
		}
	}

	void OnCollisionEnter(Collision col)
	{
		PlayerStats player_stats = col.collider.GetComponent<PlayerStats> ();
		if (player_stats != null) {
			player_stats.RecieveDamage (damage);

			this.DestroyEnemy ();
		}
	}

	void DestroyEnemy()
	{
		Vector3 posExplosion = this.transform.position;
		posExplosion.y = 5;
		explosionInstance = Instantiate (explosionParticles, posExplosion, Quaternion.identity);
		Destroy (explosionInstance, 3);
		this.GetComponent<EnemyStats> ().RecieveDamage (99999, false, false);
	}
}
