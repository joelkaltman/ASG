using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpPotionCollision : MonoBehaviour {

	public int lifeRecupers;

	private void OnTriggerEnter(Collider col)
	{
		PlayerStats stats = col.gameObject.GetComponent<PlayerStats> ();
		if (stats != null) {
			this.GetComponent<AudioSource>().Play ();

			stats.addLife (lifeRecupers);
			Destroy (this.gameObject);
		}
	}
}
