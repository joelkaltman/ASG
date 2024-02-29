using Unity.Netcode;
using UnityEngine;

public class CapCollision : ServerOnlyMonobehavior 
{
	private void OnTriggerEnter(Collider col)
	{
		PlayerStats stats = col.gameObject.GetComponent<PlayerStats> ();
		if (stats != null)
		{
			stats.Caps.Value++;
			gameObject.GetComponent<NetworkObject>()?.Despawn();
		}
	}
}
