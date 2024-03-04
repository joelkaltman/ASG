using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BatarangCollision : ServerOnlyMonobehavior {

	public List<GameObject> imagesPow;

	void OnTriggerEnter(Collider col)
	{
		if (col.gameObject.tag == "Enemy") {
			Vector3 enemyPos = col.gameObject.transform.position;
			int random = Random.Range (0, imagesPow.Count);
			var pow = Instantiate (imagesPow[random], new Vector3 (enemyPos.x, 6, enemyPos.z - 0.5f), Quaternion.Euler (50, 0, 0));
			pow.GetComponent<NetworkObject>()?.Spawn(true);
		}
	}
}
