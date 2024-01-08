using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpMultipleShootCollision : MonoBehaviour {

	public int duration;

	void OnCollisionEnter(Collision col)
	{
		PlayerGuns gun = col.collider.gameObject.GetComponentInChildren<PlayerGuns> ();
		if (gun != null) {
			this.GetComponent<AudioSource>().Play ();

			gun.ChangeShootingType (GunData.ShootingType.MULTPLE, 10);
			Destroy (this.gameObject);
		}
	}
}
