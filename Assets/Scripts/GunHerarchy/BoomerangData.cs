using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Gun/Boomerang")]

public class BoomerangData : GunData {

	[SerializeField] private bool animateThrow;
	[SerializeField] private bool comesBack;

	private GameObject player;
	private Animator anim;

	public override void Initialize () {
		this.shooter = PlayerStats.Instance.getShooter ();
		this.player = PlayerStats.Instance.getPlayer ();
		this.hand = PlayerStats.Instance.getHand ();
		this.anim = player.GetComponent<Animator> ();

		this.weaponInstance = Instantiate (weapon, this.hand.transform);
		this.weaponInstance.GetComponent<Renderer> ().enabled = false;

		this.currentCount = this.initialCount;
		this.shootingType = ShootingType.NORMAL;
	}

	public override bool Shoot ()
	{
		Animator anim = player.GetComponent<Animator> ();
		if (this.timeElapsed > this.frecuency && this.currentCount > 0 && !anim.IsInTransition(0)) {
			this.currentCount--;
			this.timeElapsed = 0;

			if (this.currentCount == 0) {
				this.weaponInstance.GetComponent<Renderer> ().enabled = false;
			}

			if(animateThrow){
				anim.SetTrigger ("Throw");
			}
			this.throwBoomerang();

			return true;
		} else {
			return false;
		}
	}

	public void throwBoomerang()
	{
		Instantiate (bullet, this.shooter.transform.position, this.shooter.transform.rotation);
	}

	public void bumerangReturned(bool addCount)
	{
		if (addCount) {
			this.currentCount++;
		}
		if (this.currentCount > 0) {
			this.weaponInstance.GetComponent<Renderer> ().enabled = true;
		}
	}

	public override void Equip ()
	{
		if (this.currentCount > 0) {
			this.weaponInstance.GetComponent<Renderer> ().enabled = true;
		}
		PlayerStats.Instance.onGranadesThrow += throwBoomerang;
	}

	public override void Discard ()
	{
		this.weaponInstance.GetComponent<Renderer> ().enabled = false;
		PlayerStats.Instance.onGranadesThrow -= throwBoomerang;
	}

	public override GunType GetGunType ()
	{
		return GunType.BOOMERANG;
	}
}
