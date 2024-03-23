using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
	public Vector3 hostSpawnPosition;
	public Vector3 clientSpawnPosition;
	
	[HideInInspector] public Joystick joystickMovement;
	[HideInInspector] public GameObject joystickMovementHolder;

	public bool IsMoving { get; private set; }
	
	float fallSpeed;
	private Rigidbody rb;
	private Animator animator;
    private PlayerStats playerStats;
    
    public bool ShouldAutoShoot { get; private set; }

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
		
		var spawnPos = IsHost ? hostSpawnPosition : clientSpawnPosition;
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

			Vector2 joystickVal = joystickMovement.CurrentValues();
			direction = new Vector3 (joystickVal.x, 0, joystickVal.y);

			float posY = Vector3.Dot (transform.forward, direction); 
			float posX = Vector3.Dot (transform.right, direction); 
			animator.SetFloat ("PosY", posY, 0.1f, Time.deltaTime);
			animator.SetFloat ("PosX", posX, 0.1f, Time.deltaTime);
		}

		var vel = rb.velocity;
		vel.x = direction.x * playerStats.Speed.Value;
		vel.z = direction.z * playerStats.Speed.Value;
		rb.velocity = vel;

		Ray ray = new(transform.position, Vector3.down);
		bool collidesWithTerrain = Physics.RaycastAll(ray, 0.5f).Any(x => x.collider.gameObject.name == "Terrain");
		if(collidesWithTerrain)
			rb.constraints |= RigidbodyConstraints.FreezePositionY;
		else
			rb.constraints &= ~RigidbodyConstraints.FreezePositionY;

		IsMoving = direction.x != 0 | direction.z != 0;
	}

	void Rotation()
	{
		// Rotate by stick
		var joystick = UIJoystickManager.Instance.CurrentJoystick;
		var playerPos = transform.position;

		if (UserManager.Instance.AimingAutomatic)
		{
			joystickMovementHolder.gameObject.SetActive(false);
			ShouldAutoShoot = false;
			
			var minmax = UIJoystickManager.Instance.CurrentType == UIJoystickManager.JoystickType.SHOOTER ? (0, 10) :
								UIJoystickManager.Instance.CurrentType == UIJoystickManager.JoystickType.GRANADE ? (2, 8) :
								(0, 6);
			
			ShouldAutoShoot = ClosestEnemyRotation(playerPos, minmax.Item1, minmax.Item2, out var rotation);
			if (ShouldAutoShoot)
				transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, 10);
		}
		else
		{
			joystickMovementHolder.gameObject.SetActive(true);
			ShouldAutoShoot = false;
			
			Vector2 joystickVal = joystick.CurrentValues();
			switch (UIJoystickManager.Instance.CurrentType)
			{
				case UIJoystickManager.JoystickType.SHOOTER:
				case UIJoystickManager.JoystickType.GRANADE:
				{
					if (SimpleJoystickRotation(joystickVal, out var rotation))
						transform.rotation = rotation;
					break;
				}
				case UIJoystickManager.JoystickType.BOOMERANG:
				{
					if (ClosestEnemyRotation(playerPos, 2, 100, out var rotation))
						transform.rotation = rotation;
					break;
				}
			}
		}
	}

	private bool SimpleJoystickRotation(Vector2 joystick, out Quaternion rotation)
	{
		rotation = Quaternion.identity;
		
		if (joystick.x != 0 && joystick.y != 0)
			return false;
		
		Vector3 dir = new Vector3 (joystick.x, 0, joystick.y) * 10;
		rotation = Quaternion.LookRotation(dir);
		return true;
	}

	private bool ClosestEnemyRotation(Vector3 playerPos, int minDistance, int maxDistance, out Quaternion rotation)
	{
		rotation = Quaternion.identity;
		
		if (!EnemiesManager.Instance.ClosestEnemyTo(playerPos, out var enemy))
			return false;

		var enemyPos = enemy.transform.position;
		var distance = Vector3.Distance(enemyPos, playerPos);
		if (distance > minDistance && distance < maxDistance)
		{
			var dir = enemyPos - playerPos;
			dir.y = 0;
			rotation = Quaternion.LookRotation(dir);
			return true;
		}

		return false;
	}
}
