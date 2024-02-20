﻿using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour {
	
	public float gravity;
	public GameObject particlesDust;
	public GameObject arrowCap;
	public GameObject joystickMovement;
	public GameObject joystickRotation;

	bool isMoving;
	float fallSpeed;
	Rigidbody rb;
	Animator animator;

	float initialY;

	// Use this for initialization
	void Start () {
		if(!IsOwner)
			return;
		
		isMoving = false;
		this.rb = this.GetComponent< Rigidbody > ();
		this.rb.maxAngularVelocity = 0;
		this.animator = this.GetComponent < Animator > ();
		this.initialY = this.transform.position.y;

		if (joystickMovement == null)
		{
			joystickMovement = GameObject.Find("MovementJoystick");
		}

		var camera = Camera.main;
		var cameraController = camera.GetComponent<CameraController>();
		cameraController.player = this.gameObject;

		//arrowCap.SetActive (true);
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		if(!IsOwner)
			return;
		
		this.FallAndMove ();

		this.RotateArrowCap ();

		if (PlayerStats.Instance.life == 0) {
			return;
		}

		this.animator.SetBool("Run", isMoving);

		this.Dust ();

		this.Rotation ();
	}

	void FallAndMove()
	{
		if (this.transform.position.y > this.initialY) {
			this.transform.position = new Vector3 (this.transform.position.x, this.initialY, this.transform.position.z);
		}

		Vector3 direction = new Vector3 ();
		if (PlayerStats.Instance.life > 0) {

			if (GameData.Instance.isMobile) {
				Vector2 joystickVal = joystickMovement.GetComponent<Joystick>().getJoystickCurrentValues();
				direction = new Vector3 (joystickVal.x, 0, joystickVal.y);
			} else {
				direction = new Vector3 (Input.GetAxis ("Horizontal"), 0, Input.GetAxis ("Vertical"));
			}

			float posY = Vector3.Dot (this.transform.forward, direction); 
			float posX = Vector3.Dot (this.transform.right, direction); 
			animator.SetFloat ("PosY", posY, 0.1f, Time.deltaTime);
			animator.SetFloat ("PosX", posX, 0.1f, Time.deltaTime);
		}

		this.rb.velocity = new Vector3(direction.x * PlayerStats.Instance.speed, this.rb.velocity.y, direction.z * PlayerStats.Instance.speed) ;

		isMoving = true;
		if (direction.x == 0 && direction.z == 0) {
			isMoving = false;
		}
	}

	void RotateArrowCap()
	{
		return;
		Vector3 capPos = PowerUpsManager.Instance.getCapPosition ();
		if (capPos == null) {
			this.arrowCap.SetActive (false);
		}else{
			this.arrowCap.SetActive (true);
			Vector3 dirCap = capPos - this.transform.position;
			dirCap.Normalize ();

			this.arrowCap.transform.LookAt (capPos, new Vector3(0,1,0));
			this.arrowCap.transform.position = new Vector3(this.transform.position.x + dirCap.x * 2, 5, this.transform.position.z + dirCap.z * 2);
		}
	}

	void Rotation()
	{
		return;
		if (Time.deltaTime > 0) {
			if (GameData.Instance.isMobile) {
				// Rotate by stick
				Vector2 joystickVal = UIJoystickManager.Instance.getCurrentJoystick().getJoystickCurrentValues ();

				UIJoystickManager.JoystickType type = UIJoystickManager.Instance.getCurrentJoystickType();
				switch (type) {
				case UIJoystickManager.JoystickType.SHOOTER:
				case UIJoystickManager.JoystickType.GRANADE:
					if (joystickVal.x != 0 && joystickVal.y != 0) {
						Vector3 dir = new Vector3 (joystickVal.x, 0, joystickVal.y) * 10;
						transform.rotation = Quaternion.LookRotation(dir);
					}
					break;
				
				case UIJoystickManager.JoystickType.BOOMERANG:
					GameObject enemy = EnemiesManager.Instance.GetClosestEnemyTo (this.transform.position);
					if (enemy != null && Vector3.Distance(enemy.transform.position, this.transform.position) > 2)
					{
						var dir = enemy.transform.position - this.transform.position;
						dir.y = 0;
						transform.rotation = Quaternion.LookRotation(dir);
					}
					break;
				}

				// Rotate when shoot granade
				/*if (Input.GetMouseButtonDown (0) && 
					(PlayerGuns.Instance.GetCurrentGun ().GetGunType () == GunData.GunType.GRANADE || PlayerGuns.Instance.GetCurrentGun ().GetGunType () == GunData.GunType.BOOMERANG)) {
					RaycastHit hit;
					Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
					if (Physics.Raycast (ray, out hit)) {

						Vector3 look_pos = hit.point - this.transform.position;
						look_pos.y = 0;
						transform.rotation = Quaternion.LookRotation (look_pos);
					}
				}*/
			} else {
				// Rotate to look mouse
				RaycastHit hit;
				Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
				if (Physics.Raycast (ray, out hit)) {

					Vector3 dir = hit.point - this.transform.position;
					dir.y = 0;
					transform.rotation = Quaternion.LookRotation (dir);
				}
			}
		}
	}

	void Dust()
	{
		if (PlayerStats.Instance.speed > 5 && isMoving) {
			GameObject dust = Instantiate (particlesDust, this.transform.position, Quaternion.identity);
			Destroy (dust, 3);
		}
	}

}
