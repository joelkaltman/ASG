using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StateMachineGiantZombie : ServerOnlyMonobehavior {

	public State idle;
	public State follow;
	public State jump;
	public State attack;

	private State currentState;

	NavMeshAgent navAgent;

	private GameObject player;

	private void Start()
	{
		idle = Instantiate (idle);
		follow = Instantiate (follow);
		jump = Instantiate (jump);
		attack = Instantiate (attack);
		
		navAgent = GetComponent<NavMeshAgent> ();
		player = MultiplayerManager.Instance.GetRandomPlayer();


		idle.Initialize (gameObject);
		follow.Initialize (gameObject);
		jump.Initialize (gameObject);
		attack.Initialize (gameObject);

		SetState(idle);
	}

	private void Update()
	{
		if (gameObject.GetComponent<EnemyStats> ().life <= 0) {
			navAgent.speed = 0;
			return;
		}

		currentState.Tick (Time.deltaTime);

		if (currentState == idle) {
			transitionIdle ();
		} else if (currentState == follow) {
			transitionFollow ();
		} else if (currentState == jump) {
			transitionJump ();
		} else if (currentState == attack) {
			transitionAttack ();
		}
	}

	void transitionIdle()
	{
		float distance = Vector3.Distance (player.transform.position, transform.position);
		if (distance >= 4) {
			SetState (jump);
		} else if (distance >= 2) {
			SetState (follow);
		} else {
			SetState (attack);
		}
	}

	void transitionFollow()
	{
		float distance = Vector3.Distance (player.transform.position, transform.position);
		if (distance <= 2.5f) {
			SetState (attack);
		}else if(distance >= 4) {
			SetState (jump);
		}
	}

	void transitionJump()
	{
		float distance = Vector3.Distance (player.transform.position, transform.position);
		if (distance <= 5 && !((StateJump)jump).isJumping()) { // ver
			SetState (follow);
		}
	}

	void transitionAttack()
	{
		float distance = Vector3.Distance (player.transform.position, transform.position);
		if (distance >= 2) {
			SetState (follow);
		}
	}

	public void SetState(State state)
	{
		if (currentState != null)
			currentState.OnStateExit();

		currentState = state;

		if (currentState != null)
			currentState.OnStateEnter();
	}

	public void EventJump()
	{
		((StateJump)jump).Jump ();
	}
}
