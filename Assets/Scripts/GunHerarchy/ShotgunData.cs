using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Gun/Shotgun")]

public class ShotgunData : GunData {

	public override void Initialize () {
		this.shooter = PlayerStats.Instance.getShooter ();
		this.hand = PlayerStats.Instance.getHand ();

		this.weaponInstance = Instantiate (weapon, this.hand.transform);
		this.weaponInstance.GetComponent<Renderer> ().enabled = false;

		this.currentCount = this.initialCount;
		this.shootingType = ShootingType.NORMAL;
	}

	public override bool Shoot ()
	{
		if (this.timeElapsed > this.frecuency && this.currentCount != 0) {
			if (this.initialCount != -1) {
				this.currentCount--;
			}
			this.timeElapsed = 0;

			switch (this.shootingType) {
			case ShootingType.NONE:
				break;
			case ShootingType.NORMAL:
				Instantiate (bullet, this.shooter.transform.position, this.shooter.transform.rotation);
				break;
			case ShootingType.MULTPLE:
				Instantiate (bullet, this.shooter.transform.position, this.shooter.transform.rotation * Quaternion.Euler (0, 0, 30));
				Instantiate (bullet, this.shooter.transform.position, this.shooter.transform.rotation);
				Instantiate (bullet, this.shooter.transform.position, this.shooter.transform.rotation * Quaternion.Euler (0, 0, -30));
				break;
			}
			return true;
		} else {
			return false;
		}
	}
		
	public override void Equip ()
	{
		this.weaponInstance.GetComponent<Renderer> ().enabled = true;
	}

	public override void Discard ()
	{
		this.weaponInstance.GetComponent<Renderer> ().enabled = false;
	}

	public override GunType GetGunType ()
	{
		return GunType.SHOTGUN;
	}
}
