using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttractEnemies : MonoBehaviour {

	public float distanceAttraction;

	public bool kill;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		List<GameObject> enemies = EnemiesManager.Instance.getInstantiatedEnemies();

		for (int i = 0; i < (enemies.Count - 1); i++) {
			float distance = Vector3.Distance (enemies [i].transform.position, this.transform.position);
			if (distance < distanceAttraction) {
				enemies [i].transform.position = Vector3.MoveTowards(enemies [i].transform.position, new Vector3(this.transform.position.x, 5, this.transform.position.z), 0.17f);
			}
		}
	}


	void OnTriggerEnter(Collider col)
	{
		if (kill) {
			EnemyStats enemyStats = col.gameObject.GetComponent<EnemyStats> ();
			if (enemyStats != null) {
				enemyStats.RecieveDamage (99999, false, true);
			}
		}
	}

}
