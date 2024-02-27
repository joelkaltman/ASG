using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapCollision : MonoBehaviour 
{
	private void OnTriggerEnter(Collider col)
	{
		PlayerStats stats = col.gameObject.GetComponent<PlayerStats> ();
		if (stats != null) {
			stats.AddCap ();
			Destroy (this.gameObject);
		}
	}
}
