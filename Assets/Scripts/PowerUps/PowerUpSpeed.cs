public class PowerUpSpeed : PowerUpCollision {

	public int duration;

	protected override void PowerUpAction(PlayerStats playerStats)
	{
		playerStats.IncreaseSpeed(2, duration);
	}
}
