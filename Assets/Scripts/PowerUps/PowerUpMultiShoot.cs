
public class PowerUpMultiShoot: PowerUpCollision {

	public int duration;

	protected override void PowerUpAction(PlayerStats playerStats)
	{
		PlayerGuns gun = playerStats.gameObject.GetComponentInChildren<PlayerGuns> ();
		gun.ChangeShootingType (GunData.ShootingType.MULTPLE, duration);
	}
}
