using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GranadeMovement : MonoBehaviour {

	// Use this for initialization
	void Start () {
		GameObject player = PlayerStats.Instance.getPlayer();

		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		if (Physics.Raycast (ray, out hit)) {
			// Calculate velocity
			float distance = Vector3.Distance(player.transform.position, hit.point);
			if (GameData.Instance.isMobile) {
				distance = 5;
			}
			float t = 1;
			float g = Physics.gravity.y;
			float vel_y = (0 - 0.4f - 0.5f * g * Mathf.Pow (t, 2)) / t;
			float vel_x = (distance - 0) / 1;

			Vector3 vel = Vector3.up * vel_y + player.transform.forward * vel_x;

			this.GetComponent<Rigidbody> ().velocity = vel;
		}
	}
}
