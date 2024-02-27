using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCollisionZombie : MonoBehaviour {

	public int damage;
	public float attackFrecuency;

	PlayerStats playerStatsInstance;

	float acumTime = 0;

	void Start()
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
			playerStatsInstance = player_stats;
			this.GetComponent<Animator> ().SetBool ("Attack", true);
			this.GetComponent<EnemyFollow> ().follow = false;
		}
	}

	void OnCollisionStay(){
		acumTime += Time.fixedDeltaTime;
		if (acumTime >= attackFrecuency && playerStatsInstance != null) {
			playerStatsInstance.RecieveDamage (damage);
			acumTime = 0;
		}
	}

	void OnCollisionExit(){
		this.GetComponent<Animator> ().SetBool ("Attack", false);
		this.GetComponent<EnemyFollow> ().follow = true;
		acumTime = 0;
		this.playerStatsInstance = null;
	}

	void AttackPlayer(){
		playerStatsInstance.RecieveDamage (damage);
	}
}
