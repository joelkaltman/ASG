using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballMovement : ServerOnlyMonobehavior {

	public float Velocity;
	
	void Update () {
		this.transform.Translate (this.transform.forward * Velocity * Time.deltaTime);
	}
}
