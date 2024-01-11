﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyFollow : MonoBehaviour {

	GameObject player;
	NavMeshAgent navAgent;

	public bool follow;

	// Use this for initialization
	void Start () {
		navAgent = this.GetComponent<NavMeshAgent> ();

		int speedMin = this.GetComponent<EnemyStats> ().speedMin;
		int speedMax = this.GetComponent<EnemyStats> ().speedMax;
		navAgent.speed = Random.Range (speedMin, speedMax + 1);

		player = PlayerStats.Instance.getPlayer ();

		this.follow = true;
	}
	
	// Update is called once per frame
	void Update () {
		if (this.gameObject.GetComponent<EnemyStats> ().life <= 0) {
			navAgent.speed = 0;
			return;
		}

		if (this.transform.position.y < 3) {
			Destroy (this.gameObject);
		}

		if (navAgent.isOnNavMesh) {
			if (this.follow) {
				navAgent.SetDestination (player.transform.position);
			} 
		} else {
			Destroy (this.gameObject);
		}
	}
}