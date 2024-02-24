using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerSpawn : NetworkBehaviour
{
    public static PlayerSpawn Instance;
    
    public GameObject playerPrefab;

    public event Action<GameObject> OnPlayerSpawn;

    private GameObject playerInstance;
    
    void Awake()
    {
        if (Instance && Instance != this)
            Destroy(Instance);

        Instance = this;
        
        playerInstance = Instantiate(playerPrefab, transform.position, Quaternion.identity);

        if (GameData.Instance.isOnline)
        {
            var netObj = playerInstance.GetComponent<NetworkObject>();
            netObj.Spawn(true);
        }
    }

    public void AddListener(Action<GameObject> onSpawnCallback)
    {
        OnPlayerSpawn += onSpawnCallback;
        if(playerInstance)
            onSpawnCallback?.Invoke(playerInstance);
    }

    public void RemoveListener(Action<GameObject> onSpawnCallback)
    {
        OnPlayerSpawn -= onSpawnCallback;
    }
}
