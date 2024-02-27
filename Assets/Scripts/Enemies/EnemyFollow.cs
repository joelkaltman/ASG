using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

public class EnemyFollow : MonoBehaviour {

	NavMeshAgent navAgent;
	private GameObject target;

	public bool follow;

	// Use this for initialization
	void Start () 
	{
		if (!MultiplayerManager.Instance.IsHostReady)
		{
			enabled = false;
			return;
		}
		
		navAgent = this.GetComponent<NavMeshAgent> ();

		int speedMin = this.GetComponent<EnemyStats> ().speedMin;
		int speedMax = this.GetComponent<EnemyStats> ().speedMax;
		navAgent.speed = Random.Range (speedMin, speedMax + 1);

		target = MultiplayerManager.Instance.GetRandomPlayer();
		
		this.follow = true;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (!target)
			return;
		
		if (this.gameObject.GetComponent<EnemyStats> ().life <= 0) {
			navAgent.speed = 0;
			return;
		}

		if (this.transform.position.y < 3) {
			//Destroy (this.gameObject);
		}

		if (navAgent.isOnNavMesh) {
			if (this.follow) {
				navAgent.SetDestination (target.transform.position);
			} 
		} else {
			//Destroy (this.gameObject);
		}
	}
}
