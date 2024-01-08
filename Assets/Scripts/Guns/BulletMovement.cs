using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMovement : MonoBehaviour {

	public float Velocity;

	// Update is called once per frame
	void Update () {
		this.transform.Translate (Vector3.up * Velocity * Time.deltaTime);
	}
}
