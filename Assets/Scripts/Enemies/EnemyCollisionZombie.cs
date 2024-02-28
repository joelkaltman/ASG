using UnityEngine;

public class EnemyCollisionZombie : ServerOnlyMonobehavior {

	public int damage;
	public float attackFrecuency;

	PlayerStats playerStatsInstance;

	float acumTime = 0;
	
	void OnCollisionEnter(Collision col)
	{
		PlayerStats player_stats = col.collider.GetComponent<PlayerStats> ();
		if (player_stats != null) {
			playerStatsInstance = player_stats;
			GetComponent<Animator> ().SetBool ("Attack", true);
			GetComponent<EnemyFollow> ().follow = false;
		}
	}

	void OnCollisionStay()
	{
		acumTime += Time.fixedDeltaTime;
		if (acumTime >= attackFrecuency && playerStatsInstance != null) {
			playerStatsInstance.RecieveDamage (damage);
			acumTime = 0;
		}
	}

	void OnCollisionExit()
	{
		GetComponent<Animator> ().SetBool ("Attack", false);
		GetComponent<EnemyFollow> ().follow = true;
		acumTime = 0;
		playerStatsInstance = null;
	}

	void AttackPlayer(){
		playerStatsInstance.RecieveDamage (damage);
	}
}
