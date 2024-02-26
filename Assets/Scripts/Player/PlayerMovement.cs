using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour {
	
	public GameObject particlesDust;
	[HideInInspector] public Joystick joystickMovement;

	bool isMoving;
	float fallSpeed;
	Rigidbody rb;
	Animator animator;

	float initialY;

    private bool shouldMove => !GameData.Instance.isOnline || IsOwner;

    private PlayerStats playerStats;

	// Use this for initialization
	void Start () {
		if(!shouldMove)
			return;
		
		isMoving = false;
		this.rb = this.GetComponent< Rigidbody > ();
		this.rb.maxAngularVelocity = 0;
		this.animator = this.GetComponent < Animator > ();
		this.initialY = this.transform.position.y;

		playerStats = GetComponent<PlayerStats>();
		//arrowCap.SetActive (true);

		this.transform.position = new Vector3(0, 5, 0);
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		if(!shouldMove)
			return;

		if (!joystickMovement)
			return;

		if (!playerStats.Initialized)
			return;
		
		this.FallAndMove ();

		if (playerStats.life == 0)
			return;

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
		if (playerStats.life > 0) {

			if (GameData.Instance.isMobile) {
				Vector2 joystickVal = joystickMovement.getJoystickCurrentValues();
				direction = new Vector3 (joystickVal.x, 0, joystickVal.y);
			} else {
				direction = new Vector3 (Input.GetAxis ("Horizontal"), 0, Input.GetAxis ("Vertical"));
			}

			float posY = Vector3.Dot (this.transform.forward, direction); 
			float posX = Vector3.Dot (this.transform.right, direction); 
			animator.SetFloat ("PosY", posY, 0.1f, Time.deltaTime);
			animator.SetFloat ("PosX", posX, 0.1f, Time.deltaTime);
		}

		this.rb.velocity = new Vector3(direction.x * playerStats.speed, this.rb.velocity.y, direction.z * playerStats.speed) ;

		isMoving = true;
		if (direction.x == 0 && direction.z == 0) {
			isMoving = false;
		}
	}

	void Rotation()
	{
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
		if (playerStats.speed > 10 && isMoving && particlesDust) {
			GameObject dust = Instantiate (particlesDust, this.transform.position, Quaternion.identity);
			Destroy (dust, 3);
		}
	}

}
