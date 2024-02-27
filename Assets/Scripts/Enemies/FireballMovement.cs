using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballMovement : MonoBehaviour {

	public float Velocity;

	void Start()
	{
		if (!MultiplayerManager.Instance.IsHostReady)
		{
			enabled = false;
			return;
		}
	}
	
	void Update () {
		this.transform.Translate (this.transform.forward * Velocity * Time.deltaTime);
	}
}
