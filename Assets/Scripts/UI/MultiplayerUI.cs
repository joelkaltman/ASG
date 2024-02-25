using System;
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

    public Action<string> OnHostStarted;
    public Action OnClientStarted;
    
    public async void StartHost()
    {
        clientPanel.SetActive(false);
        orText.SetActive(false);
        
        var result = await MultiplayerManager.Instance.StartHost();

        if (!result.Result)
        {
            Debug.LogError(result.Error);
            
            PopupUI.Instance.ShowPopUp("Error", result.Error, "Close");
            hostPanel.SetActive(true);
            orText.SetActive(true);
            return;
        }
        
        Debug.Log("-- JOIN CODE " + result.JoinCode);
        
        textJoinCodeOut.text = result.JoinCode;
        OnHostStarted?.Invoke(result.JoinCode);
    }

    public async void JoinClient()
    {
        hostPanel.SetActive(false);
        orText.SetActive(false);

        var result = await MultiplayerManager.Instance.JoinClient(textJoinCodeIn.text);

        if (!result.Result)
        {
            Debug.LogError(result.Error);
            
            PopupUI.Instance.ShowPopUp("Error", result.Error, "Close");
            hostPanel.SetActive(true);
            orText.SetActive(true);
            return;
        }
        
        OnClientStarted?.Invoke();
    }

    
    public void GoToMainMenu()
    {
        MultiplayerManager.Instance.Disconnect();
        SceneManager.LoadScene("MainMenu");
    }
}
