using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour 
{
	[HideInInspector] public Joystick joystickMovement;

	public bool IsMoving { get; private set; }
	
	float fallSpeed;
	Rigidbody rb;
	Animator animator;

    private bool shouldMove => !GameData.Instance.isOnline || IsOwner;

    private PlayerStats playerStats;

	// Use this for initialization
	public void Initialize () {
		if(!shouldMove)
			return;
		
		rb = GetComponent< Rigidbody > ();
		rb.maxAngularVelocity = 0;
		animator = GetComponent < Animator > ();

		playerStats = GetComponent<PlayerStats>();
		//arrowCap.SetActive (true);

		var spawnPos = IsHost ? new Vector3(0, 5, 0) : new Vector3(2, 5, 0);
		transform.position = spawnPos;
	}
	
	// Update is called once per frame
	void Update () {

		if(!shouldMove)
			return;

		if (!joystickMovement)
			return;
		
		FallAndMove ();

		if (playerStats.Life.Value == 0)
			return;

		animator.SetBool("Run", IsMoving);

		Rotation ();
	}

	void FallAndMove()
	{
		Vector3 direction = new Vector3 ();
		if (playerStats.Life.Value > 0) {

			Vector2 joystickVal = joystickMovement.getJoystickCurrentValues();
			direction = new Vector3 (joystickVal.x, 0, joystickVal.y);

			float posY = Vector3.Dot (transform.forward, direction); 
			float posX = Vector3.Dot (transform.right, direction); 
			animator.SetFloat ("PosY", posY, 0.1f, Time.deltaTime);
			animator.SetFloat ("PosX", posX, 0.1f, Time.deltaTime);
		}

		var vel = rb.velocity;
		var dirNormalized = direction.normalized;
		vel.x = dirNormalized.x * playerStats.Speed.Value;
		vel.z = dirNormalized.z * playerStats.Speed.Value;
		rb.velocity = vel;

		IsMoving = dirNormalized.x != 0 | dirNormalized.z != 0;
	}

	void Rotation()
	{
		if (Time.deltaTime > 0) 
		{
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
				GameObject enemy = EnemiesManager.Instance.GetClosestEnemyTo (transform.position);
				if (enemy != null && Vector3.Distance(enemy.transform.position, transform.position) > 2)
				{
					var dir = enemy.transform.position - transform.position;
					dir.y = 0;
					transform.rotation = Quaternion.LookRotation(dir);
				}
				break;
			}
		}
	}
}
