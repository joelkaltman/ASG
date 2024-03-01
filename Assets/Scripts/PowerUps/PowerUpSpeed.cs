public class PowerUpSpeed : PowerUpCollision {

	public int duration;

	protected override void PowerUpAction(PlayerStats playerStats)
	{
		playerStats.IncreaseSpeed(1.5f, duration);
	}
}
