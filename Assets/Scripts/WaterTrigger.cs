using UnityEngine;

public class WaterTrigger : ServerOnlyMonobehavior {

	PlayerStats playerStats;

	void OnTriggerEnter(Collider col)
	{
		playerStats = col.gameObject.GetComponent<PlayerStats> ();
		if (playerStats != null) {
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
