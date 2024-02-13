using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpSpeedUpCollision : MonoBehaviour {

	public int duration;

	private void OnTriggerEnter(Collider col)
	{
		PlayerStats stats = col.gameObject.GetComponent<PlayerStats> ();
		if (stats != null) {
			this.GetComponent<AudioSource>().Play ();

			stats.setSpeed (6, duration);
			Destroy (this.gameObject);
		}
	}
}
