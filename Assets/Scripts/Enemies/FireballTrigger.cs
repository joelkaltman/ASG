using UnityEngine;

public class FireballTrigger : ServerOnlyMonobehavior {

	public int damage;

	void OnTriggerEnter(Collider col)
	{
		PlayerStats stats = col.gameObject.GetComponent<PlayerStats> ();
		if (stats != null) {
			stats.RecieveDamage (damage);
		}
	}
}
