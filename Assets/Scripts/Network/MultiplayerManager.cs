using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

    public void Disconnect()
    {
        networkManager.Shutdown();
        Destroy(networkManager.gameObject);
        AuthenticationService.Instance.SignOut();
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
        if (eventType == NetworkEvent.Connect)
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

    public GameObject GetLocalPlayer()
    {
        if (networkManager == null)
            return null;
        
        return networkManager.SpawnManager?.GetLocalPlayerObject()?.gameObject;
    }
    
    public T GetLocalPlayerComponent<T>() where T : MonoBehaviour
    {
        var player = GetLocalPlayer();
        
        if (player)
            return player.GetComponent<T>();
        
        return null;
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

    public GameObject GetRandomPlayer()
    {
        var players = GetPlayers();
        var rand = UnityEngine.Random.Range(0, players.Count);
        return players[rand];
    }
}
