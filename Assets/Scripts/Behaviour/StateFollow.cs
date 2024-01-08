using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName="State/Follow")]
public class StateFollow : State {

	public int speed;

	private NavMeshAgent navAgent;

	public override void Initialize(GameObject character)
	{
		base.Initialize (character);
		navAgent = character.GetComponent<NavMeshAgent> ();
		navAgent.speed = speed;
	}

	public override void Tick(float deltaTime){
		if (navAgent.isOnNavMesh) {
			navAgent.SetDestination (target.transform.position);
		}
	}

	public override void OnStateEnter() {
		this.character.GetComponent<Animator> ().SetBool ("Walk", true);
	}


	public override void OnStateExit() {
		this.character.GetComponent<Animator> ().SetBool ("Walk", false);
	}
}
