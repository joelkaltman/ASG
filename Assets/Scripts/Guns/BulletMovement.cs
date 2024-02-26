using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BulletMovement : NetworkBehaviour {

	public float velocity;
	public int damage;
	public int destroySeconds;

	private List<string> ignoreTags = new()
	{
		"Player",
		"Projectile",
		"Gun"
	};

	void Start ()
	{
		if (!IsHost)
			return;
		
		StartCoroutine(DestroyObject());
	}

	IEnumerator DestroyObject()
	{
		yield return new WaitForSeconds(destroySeconds);
		Destroy (this.gameObject);
	}
	
	// Update is called once per frame
	void Update () {
		if (!IsHost)
			return;
		
		this.transform.Translate (Vector3.up * velocity * Time.deltaTime);
	}
	
	void OnTriggerEnter(Collider col)
	{
		if (!IsHost)
			return;
		
		if (ignoreTags.Contains(col.gameObject.tag))
			return;

		var scriptEnemyStats = col.gameObject.GetComponent<EnemyStats> ();
		if (scriptEnemyStats != null)
			scriptEnemyStats.RecieveDamage (damage, true, true);

		var sceneElement = col.gameObject.GetComponent<SceneElementCollision>();
		if(sceneElement != null)
			sceneElement.OnBulletCollision(transform.position);
		
		Destroy (this.gameObject);
	}
}
