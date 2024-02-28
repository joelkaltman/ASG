using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyStats : ServerOnlyMonobehavior {

	[HideInInspector] public UnityEvent onDieEvent;

	public int life;
	public int speedMin;
	public int speedMax;
	public bool hasAnimation;
	public GameObject vfxShoot;
	public Color vfxShootColor;
	public GameObject particlesOnDie;
	public AudioClip soundOnDie;

	bool inmuneToFire;

	private void Start()
	{
		onDieEvent = new UnityEvent ();
		inmuneToFire = false;
	}

	public void RecieveDamage(GameObject player, int damage, bool animateDying, bool addPoint)
	{
		life -= damage;
		ShowBlood();

		if (life <= 0) {
			if (addPoint && player) {
				player.GetComponent<PlayerStats>()?.AddPoints (1);
			}
			GetComponent<Collider> ().enabled = false;
			GetComponent<Rigidbody> ().useGravity = false;
			if (animateDying) {
				if (hasAnimation) {
					GetComponent<Animator> ().SetBool ("Die", true);
					Invoke ("destroy", 2);
				} else {
					GameObject particles = Instantiate (particlesOnDie, transform.position, Quaternion.Euler(-90, 0, 0));
					Destroy (particles, 1000);
					destroy ();
				}
			} else {
				destroy ();
			}

			AudioSource audio = GetComponent<AudioSource> ();
			audio.clip = soundOnDie;
			audio.Play ();

			onDieEvent.Invoke ();
		}
	}

	public void RecieveDamageByFire(GameObject player, int damage)
	{
		if (!inmuneToFire) {
			inmuneToFire = true;
			Invoke ("makeVulnerableToFire", 1);
			RecieveDamage (player, damage, true, true);
		}
	}

	private void makeVulnerableToFire()
	{
		inmuneToFire = false;
	}

	public void destroy()
	{
		EnemiesManager.Instance.DestroyEnemy (gameObject);
	}

	public void ShowBlood()
	{
		if (vfxShoot == null)
			return;
		
		GameObject blood = Instantiate (vfxShoot, transform);
		blood.transform.position += Vector3.up * 0.5f;
		var vfx = blood.GetComponent<ParticleSystem> ().main;
		vfx.startColor = new Color(vfxShootColor.r, vfxShootColor.g, vfxShootColor.b);
		Destroy (blood, 1);
	}
}
