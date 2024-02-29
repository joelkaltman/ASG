public class PowerUpPotion : PowerUpCollision {

	public int lifeRecupers;
	
	protected override void PowerUpAction(PlayerStats playerStats)
	{
		playerStats.AddLife (lifeRecupers);
	}
}
