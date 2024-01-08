using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

	public GameObject player;
	public float speedMove;
	public float speedRotate;

	Vector3 distancia;

	Vector3 initialPosition;
	Vector3 initialRotation;

	private float currentMoveSpeed;
	public float CurrentMoveSpeed{
		set{ currentMoveSpeed = value; }
	}

	// Use this for initialization
	void Start () {
		initialPosition = new Vector3 (0, 8, -5);
		initialRotation = new Vector3 (60, 0, 0);

		currentMoveSpeed = 0;
	}

	// Update is called once per frame
	void LateUpdate () {
		if (currentMoveSpeed < speedMove) {
			currentMoveSpeed += 0.1f;
		}

		this.transform.position = Vector3.MoveTowards (this.transform.position, player.transform.position + initialPosition, Time.deltaTime * PlayerStats.Instance.speed * currentMoveSpeed);

		this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.Euler(initialRotation), Time.deltaTime * speedRotate);
	}
}
