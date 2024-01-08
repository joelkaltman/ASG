using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StateMachineGiantZombie : MonoBehaviour {

	public State idle;
	public State follow;
	public State jump;
	public State attack;

	private State currentState;

	NavMeshAgent navAgent;

	private GameObject player;
	float elapsedTime;

	private void Awake()
	{
		idle = Instantiate (idle);
		follow = Instantiate (follow);
		jump = Instantiate (jump);
		attack = Instantiate (attack);
	}

	private void Start()
	{
		navAgent = this.GetComponent<NavMeshAgent> ();
		player = PlayerStats.Instance.getPlayer ();

		elapsedTime = 0;

		idle.Initialize (this.gameObject);
		follow.Initialize (this.gameObject);
		jump.Initialize (this.gameObject);
		attack.Initialize (this.gameObject);

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
		float distance = Vector3.Distance (this.player.transform.position, this.transform.position);
		if (distance >= 4) {
			elapsedTime = 0;
			this.SetState (this.jump);
		} else if (distance >= 2) {
			this.SetState (this.follow);
		} else {
			this.SetState (this.attack);
		}
	}

	void transitionFollow()
	{
		float distance = Vector3.Distance (this.player.transform.position, this.transform.position);
		if (distance <= 2.5f) {
			this.SetState (this.attack);
		}else if(distance >= 4) {
			elapsedTime = 0;
			this.SetState (this.jump);
		}
	}

	void transitionJump()
	{
		float distance = Vector3.Distance (this.player.transform.position, this.transform.position);
		if (distance <= 5 && !((StateJump)this.jump).isJumping()) { // ver
			elapsedTime = 0;
			this.SetState (this.follow);
		}
	}

	void transitionAttack()
	{
		float distance = Vector3.Distance (this.player.transform.position, this.transform.position);
		if (distance >= 2) {
			this.SetState (this.follow);
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
		((StateJump)this.jump).Jump ();
	}
}
