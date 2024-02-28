using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StateMachineGiantDemon : ServerOnlyMonobehavior {

	public float timeFarAttackMin;
	public float timeFarAttackMax;
	public State idle;
	public State follow;
	public State closeAttack;
	public State farAttack1;
	public State farAttack2;

	private float timeFarAttack;
	private State currentState;

	NavMeshAgent navAgent;

	private GameObject player;
	float elapsedTimeFarAttack;



	private void Start()
	{
		idle = Instantiate (idle);
		follow = Instantiate (follow);
		closeAttack = Instantiate (closeAttack);
		farAttack1 = Instantiate (farAttack1);
		farAttack2 = Instantiate (farAttack2);
		
		navAgent = GetComponent<NavMeshAgent> ();
		player = MultiplayerManager.Instance.GetRandomPlayer();
		
		idle.Initialize (gameObject);
		follow.Initialize (gameObject);
		closeAttack.Initialize (gameObject);
		farAttack1.Initialize (gameObject);
		farAttack2.Initialize (gameObject);

		timeFarAttack = Random.Range (timeFarAttackMin, timeFarAttackMax);

		SetState(idle);
	}

	private void Update()
	{
		if (gameObject.GetComponent<EnemyStats> ().life <= 0) {
			navAgent.speed = 0;
			return;
		}

		currentState.Tick (Time.deltaTime);

		checkFarAttack ();

		if (currentState == idle) {
			transitionIdle ();
		} else if (currentState == follow) {
			transitionFollow ();
		} else if (currentState == closeAttack) {
			transitionCloseAttack ();
		} else if (currentState == farAttack1 || currentState == farAttack2) {
			transitionFarAttack ();
		}

	}

	void checkFarAttack()
	{
		if (currentState != farAttack1 && currentState != farAttack2) {
			elapsedTimeFarAttack += Time.deltaTime;
		}

		if (elapsedTimeFarAttack > timeFarAttack && currentState != closeAttack) {
			elapsedTimeFarAttack = 0;
			timeFarAttack = Random.Range (timeFarAttackMin, timeFarAttackMax);
			int rnd = Random.Range (0, 2);
			if (rnd == 0) {
				SetState (farAttack1);
			} else {
				SetState (farAttack2);
			}
		}
	}

	void transitionIdle()
	{
		float distance = Vector3.Distance (player.transform.position, transform.position);
		if (distance < 2) {
			SetState (closeAttack);
		} else {
			SetState (follow);
		}
	}

	void transitionFollow()
	{
		float distance = Vector3.Distance (player.transform.position, transform.position);
		if (distance < 2) {
			SetState (closeAttack);
		}
	}

	void transitionCloseAttack()
	{
		float distance = Vector3.Distance (player.transform.position, transform.position);
		if (distance > 2) {
			SetState (follow);
		}
	}

	void transitionFarAttack()
	{
		float distance = Vector3.Distance (player.transform.position, transform.position);
		if (!((StateShoot)currentState).isShooting ()) {
			SetState (idle);
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


	public void EventShoot()
	{
		((StateShoot)currentState).Shoot ();
	}
}
