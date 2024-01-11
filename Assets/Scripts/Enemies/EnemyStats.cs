using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyStats : MonoBehaviour {

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

	Material material;

	private void Start()
	{
		onDieEvent = new UnityEvent ();

		material = this.gameObject.GetComponent<Material> ();

		inmuneToFire = false;
	}

	public void RecieveDamage(int damage, bool animateDying, bool addPoint)
	{
		life -= damage;
		ShowBlood();

		if (life <= 0) {
			if (addPoint) {
				PlayerStats.Instance.AddPoints (1);
			}
			this.GetComponent<Collider> ().enabled = false;
			this.GetComponent<Rigidbody> ().useGravity = false;
			if (animateDying) {
				if (hasAnimation) {
					this.GetComponent<Animator> ().SetBool ("Die", true);
					Invoke ("destroy", 2);
				} else {
					GameObject particles = Instantiate (particlesOnDie, this.transform.position, Quaternion.Euler(-90, 0, 0));
					Destroy (particles, 1000);
					this.destroy ();
				}
			} else {
				this.destroy ();
			}

			AudioSource audio = this.GetComponent<AudioSource> ();
			audio.clip = this.soundOnDie;
			audio.Play ();

			onDieEvent.Invoke ();
		}
	}

	public void RecieveDamageByFire(int damage)
	{
		if (!inmuneToFire) {
			inmuneToFire = true;
			Invoke ("makeVulnerableToFire", 1);
			this.RecieveDamage (damage, true, true);
		}
	}

	private void makeVulnerableToFire()
	{
		this.inmuneToFire = false;
	}

	public void destroy()
	{
		EnemiesManager.Instance.DestroyEnemy (this.gameObject);
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
