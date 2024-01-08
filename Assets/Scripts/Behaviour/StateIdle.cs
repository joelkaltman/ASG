using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="State/Idle")]

public class StateIdle : State {

	public override void Initialize(GameObject character)
	{
		base.Initialize (character);
	}

	public override void Tick(float deltaTime){
		

	}

	public override void OnStateEnter() {
		//this.character.GetComponent<Animator> ().SetTrigger ("Idle");
	}

	public override void OnStateExit() { 
		
	}
}
