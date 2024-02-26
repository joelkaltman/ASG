using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Gun/Granade")]

public class GranadeData : GunData {

	[SerializeField] private bool animateThrow;

	public override void Initialize (GameObject player) 
	{
		base.Initialize(player);

		this.weaponInstance = Instantiate (weapon, hand.transform);
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
				animator.SetTrigger ("Throw");
			}
			ThrowGranade ();

			return true;
		} 
		
		return false;
	}

	public void ThrowGranade()
	{
		var bulletInstance = Instantiate (bullet, shooter.transform.position, shooter.transform.rotation);
		
		var movement = bulletInstance.GetComponent<GranadeMovement>();
		if (movement)
			movement.player = player;
	}

	public override void Equip ()
	{
		if (this.currentCount > 0) {
			this.weaponInstance.GetComponent<Renderer> ().enabled = true;
		}
		playerStats.onGranadesThrow += ThrowGranade;
	}

	public override void Discard ()
	{
		this.weaponInstance.GetComponent<Renderer> ().enabled = false;
		playerStats.onGranadesThrow -= ThrowGranade;
	}

	public override GunType GetGunType ()
	{
		return GunType.GRANADE;
	}
}
