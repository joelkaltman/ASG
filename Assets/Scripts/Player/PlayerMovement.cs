using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour 
{
	[HideInInspector] public Joystick joystickMovement;

	public bool IsMoving { get; private set; }
	
	float fallSpeed;
	private Rigidbody rb;
	private Animator animator;
    private PlayerStats playerStats;

    private void Awake()
    {
	    rb = GetComponent<Rigidbody>();
	    animator = GetComponent<Animator>();
	    playerStats = GetComponent<PlayerStats>();
    }

    // Use this for initialization
	public void Initialize () 
	{
		if(!IsOwner)
			return;
		
		rb.maxAngularVelocity = 0;
		
		var spawnPos = IsHost ? new Vector3(0, 5, 0) : new Vector3(2, 5, 0);
		transform.position = spawnPos;
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		if(!IsOwner)
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

		Ray ray = new(transform.position, Vector3.down);
		bool collidesWithTerrain = Physics.RaycastAll(ray, 0.5f).Any(x => x.collider.gameObject.name == "Terrain");
		if(collidesWithTerrain)
			rb.constraints |= RigidbodyConstraints.FreezePositionY;
		else
			rb.constraints &= ~RigidbodyConstraints.FreezePositionY;

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
