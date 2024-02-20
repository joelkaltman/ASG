using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkGameManager : MonoBehaviour
{
    public void ButtonHost()
    {
        NetworkManager.Singleton.StartHost();
    }
    
    public void ButtonClient()
    {
        NetworkManager.Singleton.StartClient();
    }
}
