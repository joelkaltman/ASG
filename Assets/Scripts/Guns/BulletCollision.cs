using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCollision : MonoBehaviour {

	public int damage;

	void OnTriggerEnter(Collider col)
	{
		var scriptEnemyStats = col.gameObject.GetComponent<EnemyStats> ();
		if (scriptEnemyStats != null)
			scriptEnemyStats.RecieveDamage (damage, true, true);

		var sceneElement = col.gameObject.GetComponent<SceneElementCollision>();
		if(sceneElement != null)
			sceneElement.OnBulletCollision(transform.position);
		
		if (col.gameObject.GetComponent<PlayerMovement>() != null)
			return;
		
		Destroy (this.gameObject);
	}
}
