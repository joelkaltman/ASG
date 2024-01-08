using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapCollision : MonoBehaviour {

	void OnCollisionEnter(Collision col)
	{
		PlayerStats stats = col.collider.gameObject.GetComponent<PlayerStats> ();
		if (stats != null) {
			stats.AddCap ();
			Destroy (this.gameObject);
		}
	}
}
