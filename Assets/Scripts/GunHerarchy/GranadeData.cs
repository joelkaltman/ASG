using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Gun/Granade")]

public class GranadeData : GunData {

	[SerializeField] private bool animateThrow;

	private GameObject player;
	private Animator anim;

	public override void Initialize () {
		this.shooter = PlayerStats.Instance.getShooter ();
		this.player = PlayerStats.Instance.getPlayer();
		this.hand = PlayerStats.Instance.getHand ();
		this.anim = player.GetComponent<Animator> ();

		this.weaponInstance = Instantiate (weapon, this.hand.transform);
		this.weaponInstance.GetComponent<Renderer> ().enabled = false;

		this.currentCount = this.initialCount;
		this.shootingType = ShootingType.NORMAL;
	}

	public override bool Shoot ()
	{
		if (this.timeElapsed > this.frecuency && this.currentCount > 0) {
			this.currentCount--;
			this.timeElapsed = 0;

			if (this.currentCount == 0) {
				this.weaponInstance.GetComponent<Renderer> ().enabled = false;
			}

			if(animateThrow){
				anim.SetTrigger ("Throw");
			}
			this.throwGranade ();

			return true;
		} else {
			return false;
		}
	}

	public void throwGranade()
	{
		Instantiate (bullet, this.shooter.transform.position, this.shooter.transform.rotation);
	}

	public override void Equip ()
	{
		if (this.currentCount > 0) {
			this.weaponInstance.GetComponent<Renderer> ().enabled = true;
		}
		PlayerStats.Instance.onGranadesThrow += throwGranade;
	}

	public override void Discard ()
	{
		this.weaponInstance.GetComponent<Renderer> ().enabled = false;
		PlayerStats.Instance.onGranadesThrow -= throwGranade;
	}

	public override GunType GetGunType ()
	{
		return GunType.GRANADE;
	}
}
