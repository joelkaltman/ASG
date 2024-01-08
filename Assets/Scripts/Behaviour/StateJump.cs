using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName="State/Jump")]
public class StateJump : State {

	public GameObject dust;
	public float distanceDamage;
	public int damage;

	private Rigidbody rb;
	private NavMeshAgent nav;

	private float elapsedTime;
	private bool startedJump;

	public override void Initialize(GameObject character)
	{
		base.Initialize (character);
		this.rb = character.GetComponent<Rigidbody> ();
		this.nav = character.GetComponent<NavMeshAgent> ();
	}

	public override void Tick(float deltaTime){
		elapsedTime += deltaTime;
		if (rb.velocity.y < 0 && character.transform.position.y < 5.7 && startedJump) {

			GameObject dustInstance = Instantiate (dust, new Vector3(character.transform.position.x, 5, character.transform.position.z), Quaternion.identity);
			Destroy (dustInstance, 2);

			CameraShake scriptShake = Camera.main.GetComponent<CameraShake> ();
			if (scriptShake != null) {
				float distance = Vector3.Distance (character.transform.position, target.transform.position);
				scriptShake.Shake (0.5f, 2/distance);
			}

			elapsedTime = 0;
			startedJump = false;
		}
	}

	public void Jump()
	{			
		Vector3 direction = target.transform.position - character.transform.position;
		direction.Normalize ();
		this.character.transform.rotation = Quaternion.LookRotation (direction);

		// Jump
		float distance = Vector3.Distance(character.transform.position, target.transform.position);
		distance--;

		if (distance > 5) {
			distance = 5;
		}

		float t = 1.8f;
		float g = Physics.gravity.y;
		float vel_y = (0 - 0.4f - 0.5f * g * Mathf.Pow (t, 2)) / t;
		float vel_x = (distance - 0) / 1;

		Vector3 vel = Vector3.up * vel_y + direction * vel_x;

		rb.velocity = vel;

		startedJump = true;
	}

	public bool isJumping()
	{
		return startedJump;
	}

	public override void OnStateEnter() {
		this.character.GetComponent<Animator> ().SetBool ("Jump", true);
		this.elapsedTime = 0;
		this.startedJump = false;
		this.nav.enabled = false;
	}

	public override void OnStateExit() {
		this.character.GetComponent<Animator> ().SetBool ("Jump", false);
		nav.enabled = true;
		rb.velocity = new Vector3 (0, 0, 0);

		if (Vector3.Distance (this.character.transform.position, this.target.transform.position) < this.distanceDamage) {
			PlayerStats ps = this.target.GetComponent<PlayerStats> ();
			if (ps != null) {
				ps.RecieveDamage (this.damage);
			}
		}
	}
}
