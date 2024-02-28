using UnityEngine;
using UnityEngine.AI;

public class EnemyFollow : ServerOnlyMonobehavior {

	NavMeshAgent navAgent;
	public bool follow;

	// Use this for initialization
	void Start () 
	{
		navAgent = GetComponent<NavMeshAgent> ();

		int speedMin = GetComponent<EnemyStats> ().speedMin;
		int speedMax = GetComponent<EnemyStats> ().speedMax;
		navAgent.speed = Random.Range (speedMin, speedMax + 1);
		
		follow = true;
	}
	
	// Update is called once per frame
	void Update ()
	{
		var target = MultiplayerManager.Instance.GetPlayerCloserTo(transform.position);
		
		if (!target)
			return;
		
		if (gameObject.GetComponent<EnemyStats> ().life <= 0) {
			navAgent.speed = 0;
			return;
		}

		if (transform.position.y < 3) {
			//Destroy (gameObject);
		}

		if (navAgent.isOnNavMesh) {
			if (follow) {
				navAgent.SetDestination (target.transform.position);
			} 
		}
	}
}
