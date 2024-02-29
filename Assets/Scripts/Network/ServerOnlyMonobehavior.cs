using UnityEngine;

public class ServerOnlyMonobehavior : MonoBehaviour
{
    void Awake()
    {
        if(!MultiplayerManager.Instance.IsHostReady)
            Destroy(this);
    }
}
