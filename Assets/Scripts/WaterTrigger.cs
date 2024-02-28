using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterTrigger : MonoBehaviour {

	PlayerStats playerStats;

	void OnTriggerEnter(Collider col)
	{
		playerStats = col.gameObject.GetComponent<PlayerStats> ();
		if (playerStats != null) {
			//stats.RecieveDamage (99999);
			Invoke ("KillPlayer", 1);
		}
		EnemyStats enemyStats = col.gameObject.GetComponent<EnemyStats> ();
		if (enemyStats != null) {
			enemyStats.RecieveDamage (null, 99999, true, false);
		}
	}

	private void KillPlayer(){
		playerStats.RecieveDamage (99999);
	}
}
