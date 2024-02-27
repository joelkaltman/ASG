using Unity.Netcode;
using UnityEngine;

public class GranadeMovement : NetworkBehaviour 
{
	[HideInInspector] public GameObject player;
	
	void Start () 
	{
		if (!IsHost)
		{
			enabled = false;
			return;
		}
		
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		if (Physics.Raycast (ray, out var hit)) {
			// Calculate velocity
			float distance = 5;
			float t = 1;
			float g = Physics.gravity.y;
			float vel_y = (0 - 0.4f - 0.5f * g * Mathf.Pow (t, 2)) / t;
			float vel_x = (distance - 0) / 1;

			Vector3 vel = Vector3.up * vel_y + player.transform.forward * vel_x;

			GetComponent<Rigidbody> ().velocity = vel;
		}
	}
}
