using System;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MultiplayerUI : MonoBehaviour
{
    [Header("Host")] 
    public GameObject hostPanel;
    public Text textJoinCodeOut;
    
    [Header("Client")] 
    public GameObject clientPanel;
    public Text textJoinCodeIn;

    [Header("Other")] 
    public GameObject orText;
    public GameObject playerSpawn;
    
    private NetworkManager networkManager;
    private UnityTransport unityTransport;
    
    async void Start()
    {
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        networkManager = NetworkManager.Singleton;
        unityTransport = networkManager.GetComponent<UnityTransport>();
    }

    public async void StartHost()
    {
        clientPanel.SetActive(false);
        orText.SetActive(false);
        try
        {
            var allocation = await RelayService.Instance.CreateAllocationAsync(1);
            var joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            textJoinCodeOut.text = joinCode;

            var relayServerData = new RelayServerData(allocation, "dtls");
            unityTransport.SetRelayServerData(relayServerData);
            unityTransport.OnTransportEvent += TransportEvent;
            networkManager.StartHost();
        }
        catch (RelayServiceException e)
        {
            PopupUI.Instance.ShowPopUp("Error", e.Message, "Close");
            clientPanel.SetActive(true);
            orText.SetActive(true);
        }
    }

    public async void JoinClient()
    {
        hostPanel.SetActive(false);
        orText.SetActive(false);
        try
        {
            string joinCode = textJoinCodeIn.text;
            
            var allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            
            var relayServerData = new RelayServerData(allocation, "dtls");
            unityTransport.SetRelayServerData(relayServerData);
            unityTransport.OnTransportEvent += TransportEvent;
            networkManager.StartClient();
        }
        catch (RelayServiceException e)
        {
            Debug.LogError(e.Message);
            
            PopupUI.Instance.ShowPopUp("Error", e.Message, "Close");
            hostPanel.SetActive(true);
            orText.SetActive(true);
        }
    }

    private void TransportEvent(NetworkEvent eventType, ulong clientId, ArraySegment<byte> payload, float receiveTime)
    {
        if (eventType == NetworkEvent.Connect)
        {
            StartGame();
        }
    }
    
    public void StartGame()
    {
        GameData.Instance.isOnline = true;

        if (networkManager.IsHost)
        {
            var spawner = Instantiate(playerSpawn);
            spawner.GetComponent<NetworkObject>().Spawn();
            spawner.GetComponent<PlayerSpawn>().Initialize();
            networkManager.SceneManager.LoadScene("Game", LoadSceneMode.Single);
        }
    }
    
    public void GoToMainMenu()
    {
        networkManager.Shutdown();
        Destroy(networkManager);
        SceneManager.LoadScene("MainMenu");
    }
}
