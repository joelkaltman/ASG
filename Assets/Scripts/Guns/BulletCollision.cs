using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCollision : MonoBehaviour {

	public int damage;

	void OnCollisionEnter(Collision col)
	{
		EnemyStats scriptEnemyStats = col.gameObject.GetComponent<EnemyStats> ();
		if (scriptEnemyStats != null) {
			scriptEnemyStats.RecieveDamage (damage, true, true);
		}
		Destroy (this.gameObject);
	}
}
