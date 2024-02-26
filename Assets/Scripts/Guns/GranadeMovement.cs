using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GranadeMovement : MonoBehaviour 
{
	[HideInInspector] public GameObject player;
	
	void Start () {
		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		if (Physics.Raycast (ray, out hit)) {
			// Calculate velocity
			float distance = 5;
			float t = 1;
			float g = Physics.gravity.y;
			float vel_y = (0 - 0.4f - 0.5f * g * Mathf.Pow (t, 2)) / t;
			float vel_x = (distance - 0) / 1;

			Vector3 vel = Vector3.up * vel_y + player.transform.forward * vel_x;

			this.GetComponent<Rigidbody> ().velocity = vel;
		}
	}
}
