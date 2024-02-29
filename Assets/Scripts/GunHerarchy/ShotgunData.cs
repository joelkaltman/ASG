using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(menuName="Gun/Shotgun")]

public class ShotgunData : GunData {

	public override void Initialize (GameObject player)
	{
		base.Initialize(player);

		weaponInstance = Instantiate (weapon, hand.transform);
		weaponInstance.GetComponent<Renderer>().enabled = false;

		currentCount = initialCount;
	}

	public override bool Shoot ()
	{
		if (timeElapsed > frecuency && currentCount != 0) {
			if (initialCount != -1) {
				currentCount--;
			}
			timeElapsed = 0;

			switch (playerGuns.ShootType.Value)
			{
				case ShootingType.NONE:
				{
					break;
				}
				case ShootingType.NORMAL:
				{
					shooter.transform.GetPositionAndRotation(out var pos, out var rot);
					playerGuns.ShootServerRpc(Id, pos, rot);
					break;
				}
				case ShootingType.MULTPLE:
				{
					shooter.transform.GetPositionAndRotation(out var pos, out var rot);
					playerGuns.ShootServerRpc(Id, pos, rot * Quaternion.Euler(0, 0, 30));
					playerGuns.ShootServerRpc(Id, pos, rot);
					playerGuns.ShootServerRpc(Id, pos, rot * Quaternion.Euler(0, 0, -30));
					break;
				}
			}

			return true;
		}

		return false;
	}
		
	public override void Equip ()
	{
		weaponInstance.GetComponent<Renderer>().enabled = true;
	}

	public override void Discard ()
	{
		weaponInstance.GetComponent<Renderer>().enabled = false;
	}

	public override GunType GetGunType ()
	{
		return GunType.SHOTGUN;
	}
}
