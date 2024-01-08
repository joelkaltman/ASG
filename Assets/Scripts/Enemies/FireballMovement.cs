using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballMovement : MonoBehaviour {

	public float Velocity;

	// Update is called once per frame
	void Update () {
		this.transform.Translate (this.transform.forward * Velocity * Time.deltaTime);
	}
}
