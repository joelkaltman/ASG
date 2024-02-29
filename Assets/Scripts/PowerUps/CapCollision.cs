public class CapCollision : PowerUpCollision 
{
	protected override void PowerUpAction(PlayerStats playerStats)
	{
		playerStats.Caps.Value++;
	}
}
