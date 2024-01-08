using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballTrigger : MonoBehaviour {

	public int damage;

	void OnTriggerEnter(Collider col)
	{
		PlayerStats stats = col.gameObject.GetComponent<PlayerStats> ();
		if (stats != null) {
			stats.RecieveDamage (damage);
		}
	}
}
