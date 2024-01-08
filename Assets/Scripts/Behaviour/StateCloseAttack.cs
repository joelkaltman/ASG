using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="State/CloseAttack")]
public class StateCloseAttack : State {

	public float duration;
	public int damage;
	public bool throwTarget;

	private Rigidbody rbTarget;

	private float elapsedTime;

	public override void Initialize(GameObject character)
	{
		base.Initialize (character);
		this.rbTarget = target.GetComponent<Rigidbody> ();
	}

	public override void Tick(float deltaTime){
		elapsedTime += deltaTime;

		if (elapsedTime >= duration) {
			elapsedTime = 0;

			PlayerStats ps = this.target.GetComponent<PlayerStats> ();
			if (ps != null) {
				ps.RecieveDamage (this.damage);
			}

			if (throwTarget) {
				Vector3 dir = target.transform.position - character.transform.position;
				Vector3 dirPunch = new Vector3 (dir.x, 5, dir.z);
				rbTarget.AddForce (dirPunch);
			}
		}
	}

	public override void OnStateEnter() {
		this.character.GetComponent<Animator> ().SetBool ("CloseAttack", true);
		this.elapsedTime = 0;
	}

	public override void OnStateExit() {
		this.character.GetComponent<Animator> ().SetBool ("CloseAttack", false);
	}
}
