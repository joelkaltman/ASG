using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Multiplayer.Samples.Utilities.ClientAuthority;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using UnityEngine;

public class MultiplayerManager : MonoBehaviour
{
    public static MultiplayerManager Instance;
    
    public GameObject networkManagerSP;
    public GameObject networkManagerMP;

    public Vector3 spawnPos = new (0, 5, 0);
        
    private NetworkManager networkManager;
    private UnityTransport unityTransport;

    [HideInInspector] public bool IsGameReady;
    [HideInInspector] public bool IsLocalPlayerReady;
    
    public event Action OnGameReady;
    public event Action<GameObject> OnLocalPlayerReady;

    public bool IsHostReady => IsGameReady && (networkManager ? networkManager.IsHost : false);
    
    void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }
    
    public void InitializeSinglePlayer()
    {
        var sp = Instantiate(networkManagerSP);
        networkManager = sp.GetComponent<NetworkManager>();
        unityTransport = sp.GetComponent<UnityTransport>();
        networkManager.StartHost();

        IsLocalPlayerReady = true;
        var player = networkManager.SpawnManager.GetLocalPlayerObject();
        OnLocalPlayerReady?.Invoke(player.gameObject);
        
        StartGame();
    }
    
    public async void InitializeMultiplayer()
    {
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        var mp = Instantiate(networkManagerMP);
        networkManager = mp.GetComponent<NetworkManager>();
        unityTransport = mp.GetComponent<UnityTransport>();
    }

    public struct ConnectionResult
    {
        public bool Result;
        public string JoinCode;
        public string Error;
    }
    
    public async Task<ConnectionResult> StartHost()
    {
        try
        {
            var allocation = await RelayService.Instance.CreateAllocationAsync(2);
            var joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            
            var relayServerData = new RelayServerData(allocation, "dtls");
            unityTransport.SetRelayServerData(relayServerData);
            unityTransport.OnTransportEvent += TransportEvent;
            networkManager.StartHost();
            
            var player = networkManager.SpawnManager.GetLocalPlayerObject();
            player.transform.position = spawnPos;
            
            return new ConnectionResult() { Result = true, JoinCode = joinCode };
        }
        catch (RelayServiceException e)
        {
            return new ConnectionResult() { Result = false, Error = e.Message };
        }
    }

    public async Task<ConnectionResult> JoinClient(string joinCode)
    {
        try
        {
            var allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            
            var relayServerData = new RelayServerData(allocation, "dtls");
            unityTransport.SetRelayServerData(relayServerData);
            unityTransport.OnTransportEvent += TransportEvent;
            networkManager.StartClient();

            return new ConnectionResult() { Result = true, JoinCode = joinCode };
        }
        catch (RelayServiceException e)
        {
            return new ConnectionResult() { Result = false, Error = e.Message };
        }
    }

    private void TransportEvent(NetworkEvent eventType, ulong clientId, ArraySegment<byte> payload, float receiveTime)
    {
        if (networkManager.IsHost && eventType == NetworkEvent.Connect)
        {
            StartGame();
        }
    }

    public void RegisterPlayer(GameObject player)
    {
        var netObject = player.GetComponent<NetworkObject>();
        if (netObject.IsLocalPlayer)
        {
            IsLocalPlayerReady = true;
            OnLocalPlayerReady?.Invoke(player);
        }
    }
    
    public void StartGame()
    {
        IsGameReady = true;
        OnGameReady?.Invoke();
    }

    public void Disconnect()
    {
        networkManager.Shutdown();
        Destroy(networkManager);
    }

    public List<GameObject> GetPlayers()
    {
        var players = new List<GameObject>();

        if (networkManager == null)
            return players;
        
        foreach (var clientId in networkManager.ConnectedClientsIds)
        {
            var netPlayer = networkManager.SpawnManager.GetPlayerNetworkObject(clientId);
            players.Add(netPlayer.gameObject);
        }

        return players;
    }
}
