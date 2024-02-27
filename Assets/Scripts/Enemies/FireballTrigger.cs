using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballTrigger : MonoBehaviour {

	public int damage;

	private void Start()
	{
		if (!MultiplayerManager.Instance.IsHostReady)
		{
			enabled = false;
			return;
		}
	}

	void OnTriggerEnter(Collider col)
	{
		PlayerStats stats = col.gameObject.GetComponent<PlayerStats> ();
		if (stats != null) {
			stats.RecieveDamage (damage);
		}
	}
}
