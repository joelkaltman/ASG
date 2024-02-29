using Unity.Netcode;
using UnityEngine;

public abstract class PowerUpCollision : ServerOnlyMonobehavior 
{
    private void OnTriggerEnter(Collider col)
    {
        PlayerStats stats = col.gameObject.GetComponent<PlayerStats> ();
        if (stats != null) {
            GetComponent<AudioSource>().Play ();
            PowerUpAction(stats);
            gameObject.GetComponent<NetworkObject>()?.Despawn();
        }
    }

    protected abstract void PowerUpAction(PlayerStats playerStats);
}
