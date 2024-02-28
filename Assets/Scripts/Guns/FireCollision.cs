using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireCollision : PlayerOwned {

	void OnTriggerStay(Collider col)
	{
		PlayerStats player_stats = col.gameObject.GetComponent<PlayerStats> ();
		if (player_stats != null) {
			player_stats.RecieveDamageByFire (10);
		}
		EnemyStats enemy_stats = col.gameObject.GetComponent<EnemyStats> ();
		if (enemy_stats != null) {
			enemy_stats.RecieveDamageByFire (player, 10);
		}
	}
}
