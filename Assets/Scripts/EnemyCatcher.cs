using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCatcher : MonoBehaviour {

	void OnTriggerEnter(Collider col)
	{
		if (col.gameObject.tag == "Enemy") {
			GameObject player = PlayerStats.Instance.getPlayer ();
			Vector3 dir = player.transform.position - col.gameObject.transform.position;
			dir.y = 0;
			dir.Normalize ();

			float radius = this.GetComponent<SphereCollider> ().radius;

			col.gameObject.transform.Translate (dir * 15);
		}
	}
}
