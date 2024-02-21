using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.UI;

public class NetworkGameManager : MonoBehaviour
{
    public UnityTransport unityTransport;
    
    public Text textIp;
    public Text textPort;
    
    public void ButtonHost()
    {
        //unityTransport.ConnectionData.Address = textIp.text;
        //unityTransport.ConnectionData.Port = (ushort)Int32.Parse(textPort.text);
        
        NetworkManager.Singleton.StartHost();
    }
    
    public void ButtonClient()
    {
        //unityTransport.ConnectionData.Address = textIp.text;
        //unityTransport.ConnectionData.Port = (ushort)Int32.Parse(textPort.text);
        
        NetworkManager.Singleton.StartClient();
    }
}
