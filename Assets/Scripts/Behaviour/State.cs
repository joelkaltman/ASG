using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State : ScriptableObject {

	protected GameObject character;
	protected GameObject target;

	public virtual void Initialize(GameObject character)
	{
		this.character = character;
		this.target = PlayerStats.Instance.getPlayer ();
	}
		
	public abstract void Tick (float deltaTime);

	public virtual void OnStateEnter() { }
	public virtual void OnStateExit() { }

}
