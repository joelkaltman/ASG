using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCollision : MonoBehaviour {

	public int damage;

	void OnTriggerEnter(Collider col)
	{
		EnemyStats scriptEnemyStats = col.gameObject.GetComponent<EnemyStats> ();
		if (scriptEnemyStats != null) {
			scriptEnemyStats.RecieveDamage (damage, true, true);
		}

		if (col.gameObject.GetComponent<PlayerMovement>() != null)
			return;
		
		Destroy (this.gameObject);
	}
}
