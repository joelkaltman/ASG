using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCollisionVoodoo : EnemyAttack {

	public int damage;
	public GameObject explosionParticles;

	GameObject explosionInstance;

	/*void OnCollisionEnter(Collision col)
	{
		PlayerStats player_stats = col.collider.GetComponent<PlayerStats> ();
		if (player_stats != null) {
			player_stats.RecieveDamage (damage);

			this.DestroyEnemy ();
		}
	}*/

	protected override void OnEnterAttackRange(GameObject player)
	{
		player.GetComponent<PlayerStats>().RecieveDamage (damage);
		
		Vector3 posExplosion = this.transform.position;
		posExplosion.y = 5;
		explosionInstance = Instantiate (explosionParticles, posExplosion, Quaternion.identity);
		Destroy (explosionInstance, 3);
		this.GetComponent<EnemyStats> ().RecieveDamage (player, 99999, false, false);
	}

	protected override void OnStayAttackRange(GameObject player)
	{
	}

	protected override void OnExitAttackRange()
	{
	}
}
