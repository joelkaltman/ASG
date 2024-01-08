using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName="State/Shoot")]
public class StateShoot : State {

	public enum ShootingType
	{
		LINEAR,
		PARABOLIC,
		EXPANSIVE
	};

	public ShootingType type;
	public float duration;
	public int bulletsCountPerShoot;
	public int bulletsAngle;
	public float bulletVelocity;
	public GameObject bullet;

	private float elapsedTime;
	private float elapsedFrecuencyTime;

	private bool shooting;
	private NavMeshAgent nav;
	private float originalSpeed;

	public override void Initialize(GameObject character)
	{
		base.Initialize (character);
		this.nav = character.GetComponent<NavMeshAgent> ();
	}

	public override void Tick(float deltaTime){
		elapsedTime += deltaTime;

		if (elapsedTime >= duration) {
			shooting = false;
		}
	}

	public void Shoot(){
		Vector3 pos = new Vector3 (character.transform.position.x, 5.5f, character.transform.position.z);

		for (int i = -bulletsCountPerShoot / 2; i <= (bulletsCountPerShoot / 2) + 1; i++) {
			Quaternion rot = character.transform.rotation * Quaternion.Euler (0, i * bulletsAngle, 0);
			GameObject instance = Instantiate(bullet, pos, rot);

			switch (type) {
			case ShootingType.LINEAR:
				instance.GetComponent<Rigidbody> ().velocity = instance.transform.forward * bulletVelocity;
				break;
			case ShootingType.PARABOLIC:
				instance.GetComponent<Rigidbody> ().useGravity = true;
				instance.GetComponent<Rigidbody> ().velocity = instance.transform.forward * bulletVelocity;
				instance.GetComponent<Rigidbody> ().AddForce (0, 300, 0);
				break;
			}
		}
	}

	public override void OnStateEnter() {
		this.character.GetComponent<Animator> ().SetBool ("Shoot", true);
		this.elapsedTime = 0;
		this.elapsedFrecuencyTime = 0;
		this.shooting = true;

		this.originalSpeed = this.nav.speed;
		this.nav.speed = 0;
	}

	public override void OnStateExit() {
		this.character.GetComponent<Animator> ().SetBool ("Shoot", false);
		this.nav.speed = this.originalSpeed;
	}

	public bool isShooting()
	{
		return shooting;
	}
}
