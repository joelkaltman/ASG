using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtPlayer : MonoBehaviour {

	GameObject player;

	// Use this for initialization
	void Start () {
		player = PlayerStats.Instance.getPlayer ();
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 dir = player.transform.position - this.transform.position;
		this.transform.rotation = Quaternion.LookRotation (dir);
	}
}
