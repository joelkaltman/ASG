using System;
using Unity.Multiplayer.Samples.Utilities.ClientAuthority;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class PlayerSpawn : NetworkBehaviour
{
    public static PlayerSpawn Instance;
    
    public GameObject playerPrefab;
    public GameObject networkManager;

    public event Action<GameObject> OnPlayerSpawn;

    private GameObject playerInstance;
    
    void Awake()
    {
        if (Instance && Instance != this)
            Destroy(Instance);

        Instance = this;

        if (!GameData.Instance.isOnline)
        {
            var netManager = Instantiate(networkManager);
            netManager.GetComponent<NetworkManager>().StartHost();
        }
        
        playerInstance = Instantiate(playerPrefab, transform.position, Quaternion.identity);
        var netObj = playerInstance.GetComponent<NetworkObject>();
        netObj.Spawn(true);
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
