using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCollision : PlayerOwned {

	public int damage;

	private List<string> ignoreTags = new()
	{
		"Player",
		"Projectile",
		"Gun"
	};

	void OnTriggerEnter(Collider col)
	{
		if (ignoreTags.Contains(col.gameObject.tag))
			return;

		var scriptEnemyStats = col.gameObject.GetComponent<EnemyStats> ();
		if (scriptEnemyStats != null)
			scriptEnemyStats.RecieveDamage (player, damage, true, true);

		var sceneElement = col.gameObject.GetComponent<SceneElementCollision>();
		if(sceneElement != null)
			sceneElement.OnBulletCollision(transform.position);
		
		Destroy (gameObject);
	}
}
