using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpPotionCollision : MonoBehaviour {

	public int lifeRecupers;

	void OnCollisionEnter(Collision col)
	{
		PlayerStats stats = col.collider.gameObject.GetComponent<PlayerStats> ();
		if (stats != null) {
			this.GetComponent<AudioSource>().Play ();

			stats.addLife (lifeRecupers);
			Destroy (this.gameObject);
		}
	}
}
