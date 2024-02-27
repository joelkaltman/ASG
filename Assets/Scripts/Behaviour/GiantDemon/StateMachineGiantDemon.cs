using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StateMachineGiantDemon : MonoBehaviour {

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
	float elapsedTime;
	float elapsedTimeFarAttack;


	private void Awake()
	{
		idle = Instantiate (idle);
		follow = Instantiate (follow);
		closeAttack = Instantiate (closeAttack);
		farAttack1 = Instantiate (farAttack1);
		farAttack2 = Instantiate (farAttack2);
	}

	private void Start()
	{
		if (!MultiplayerManager.Instance.IsHostReady)
		{
			enabled = false;
			return;
		}
		
		navAgent = this.GetComponent<NavMeshAgent> ();
		player = MultiplayerManager.Instance.GetRandomPlayer();

		elapsedTime = 0;

		idle.Initialize (this.gameObject);
		follow.Initialize (this.gameObject);
		closeAttack.Initialize (this.gameObject);
		farAttack1.Initialize (this.gameObject);
		farAttack2.Initialize (this.gameObject);

		timeFarAttack = Random.Range (timeFarAttackMin, timeFarAttackMax);

		SetState(this.idle);
	}

	private void Update()
	{
		if (this.gameObject.GetComponent<EnemyStats> ().life <= 0) {
			navAgent.speed = 0;
			return;
		}

		elapsedTime += Time.deltaTime;

		this.currentState.Tick (Time.deltaTime);

		this.checkFarAttack ();

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

		if (elapsedTimeFarAttack > timeFarAttack && currentState != this.closeAttack) {
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
		float distance = Vector3.Distance (this.player.transform.position, this.transform.position);
		if (distance < 2) {
			SetState (this.closeAttack);
		} else {
			SetState (this.follow);
		}
	}

	void transitionFollow()
	{
		float distance = Vector3.Distance (this.player.transform.position, this.transform.position);
		if (distance < 2) {
			SetState (this.closeAttack);
		}
	}

	void transitionCloseAttack()
	{
		float distance = Vector3.Distance (this.player.transform.position, this.transform.position);
		if (distance > 2) {
			SetState (this.follow);
		}
	}

	void transitionFarAttack()
	{
		float distance = Vector3.Distance (this.player.transform.position, this.transform.position);
		if (!((StateShoot)this.currentState).isShooting ()) {
			SetState (this.idle);
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
		((StateShoot)this.currentState).Shoot ();
	}
}
