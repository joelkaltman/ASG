using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpMultipleShootCollision : MonoBehaviour {

	public int duration;

	private void OnTriggerEnter(Collider col)
	{
		PlayerGuns gun = col.gameObject.GetComponentInChildren<PlayerGuns> ();
		if (gun != null) {
			this.GetComponent<AudioSource>().Play ();

			gun.ChangeShootingType (GunData.ShootingType.MULTPLE, duration);
			Destroy (this.gameObject);
		}
	}
}
