using UnityEngine;
using UnityEngine.AI;

public class EnemyFollow : ServerOnlyMonobehavior {

	private NavMeshAgent navAgent;
	private EnemyStats enemyStats;
	[HideInInspector] public bool follow;

	// Use this for initialization
	void Start () 
	{
		navAgent = GetComponent<NavMeshAgent>();
		enemyStats = GetComponent<EnemyStats>();

		int speedMin = GetComponent<EnemyStats>().speedMin;
		int speedMax = GetComponent<EnemyStats>().speedMax;
		navAgent.speed = Random.Range (speedMin, speedMax + 1);
		
		follow = true;
	}
	
	// Update is called once per frame
	void Update ()
	{
		var target = MultiplayerManager.Instance.GetPlayerCloserTo(transform.position);
		
		if (!target)
			return;
		
		if (enemyStats.life <= 0) {
			navAgent.speed = 0;
			enabled = false;
			return;
		}

		if (navAgent.isOnNavMesh && follow) {
			navAgent.SetDestination (target.transform.position);
		}
	}
}
