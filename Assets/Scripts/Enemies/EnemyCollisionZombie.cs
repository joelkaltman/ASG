using System;
using UnityEngine;

public class EnemyCollisionZombie : ServerOnlyMonobehavior {

	public int damage;
	public float attackFrecuency;

	PlayerStats playerStatsInstance;

	float acumTime = 0;

	private Animator animator;
	private EnemyFollow enemyFollow;

	private void Start()
	{
		animator = GetComponent<Animator>();
		enemyFollow = GetComponent<EnemyFollow>();
	}

	void OnCollisionEnter(Collision col)
	{
		PlayerStats playerStats = col.collider.GetComponent<PlayerStats> ();
		if (playerStats != null) {
			playerStatsInstance = playerStats;
			animator.SetBool ("Attack", true);
			enemyFollow.follow = false;
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
		animator.SetBool ("Attack", false);
		enemyFollow.follow = true;
		acumTime = 0;
		playerStatsInstance = null;
	}

	void AttackPlayer(){
		playerStatsInstance.RecieveDamage (damage);
	}
}
