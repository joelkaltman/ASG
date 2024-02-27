﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Gun/Granade")]

public class GranadeData : GunData {

	[SerializeField] private bool animateThrow;

	public override void Initialize (GameObject player) 
	{
		base.Initialize(player);

		weaponInstance = Instantiate (weapon, hand.transform);
		weaponInstance.GetComponent<Renderer> ().enabled = false;

		currentCount = initialCount;
		shootingType = ShootingType.NORMAL;
	}

	public override bool Shoot ()
	{
		if (timeElapsed > frecuency && currentCount > 0) {
			currentCount--;
			timeElapsed = 0;

			if (currentCount == 0) {
				weaponInstance.GetComponent<Renderer> ().enabled = false;
			}

			if(animateThrow){
				animator.SetTrigger ("Throw");
			}
			ThrowGrenade ();

			return true;
		} 
		
		return false;
	}

	private void ThrowGrenade()
	{
		shooter.transform.GetPositionAndRotation(out var pos, out var rot);
		playerGuns.ThrowGrenadeServerRpc(Id, pos, rot);
	}

	public override void Equip ()
	{
		if (currentCount > 0) {
			weaponInstance.GetComponent<Renderer> ().enabled = true;
		}
		playerStats.onGranadesThrow += ThrowGrenade;
	}

	public override void Discard ()
	{
		weaponInstance.GetComponent<Renderer> ().enabled = false;
		playerStats.onGranadesThrow -= ThrowGrenade;
	}

	public override GunType GetGunType ()
	{
		return GunType.GRANADE;
	}
}
