using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Gun/Boomerang")]

public class BoomerangData : GunData {

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
		if (timeElapsed > frecuency && currentCount > 0 && !animator.IsInTransition(0)) {
			currentCount--;
			timeElapsed = 0;

			if (currentCount == 0) {
				weaponInstance.GetComponent<Renderer> ().enabled = false;
			}

			if(animateThrow){
				animator.SetTrigger ("Throw");
			}
			ThrowBoomerang();

			return true;
		} 
		
		return false;
	}

	private void ThrowBoomerang()
	{
		shooter.transform.GetPositionAndRotation(out var pos, out var rot);
		playerGuns.ThrowBoomerangServerRpc(Id, pos, rot);
	}

	public void BoomerangReturned(bool addCount)
	{
		if (addCount) {
			currentCount++;
		}
		if (currentCount > 0) {
			weaponInstance.GetComponent<Renderer> ().enabled = true;
		}
	}

	public override void Equip ()
	{
		if (currentCount > 0) {
			weaponInstance.GetComponent<Renderer> ().enabled = true;
		}
		playerStats.onGranadesThrow += ThrowBoomerang;
	}

	public override void Discard ()
	{
		weaponInstance.GetComponent<Renderer> ().enabled = false;
		playerStats.onGranadesThrow -= ThrowBoomerang;
	}

	public override GunType GetGunType ()
	{
		return GunType.BOOMERANG;
	}
}
