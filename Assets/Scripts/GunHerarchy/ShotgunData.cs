using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
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
				var spawnedBullet = Instantiate (bullet, this.shooter.transform.position, this.shooter.transform.rotation);
				spawnedBullet.GetComponent<NetworkObject>().Spawn();
				break;
			case ShootingType.MULTPLE:
				var spawnedBullet1 = Instantiate (bullet, this.shooter.transform.position, this.shooter.transform.rotation * Quaternion.Euler (0, 0, 30));
				var spawnedBullet2 = Instantiate (bullet, this.shooter.transform.position, this.shooter.transform.rotation);
				var spawnedBullet3 = Instantiate (bullet, this.shooter.transform.position, this.shooter.transform.rotation * Quaternion.Euler (0, 0, -30));
				
				spawnedBullet1.GetComponent<NetworkObject>().Spawn();
				spawnedBullet2.GetComponent<NetworkObject>().Spawn();
				spawnedBullet3.GetComponent<NetworkObject>().Spawn();
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
