using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MultiplayerUI : MonoBehaviour
{
    [Header("Host")] 
    public GameObject hostPanel;
    public Button hostButton;
    
    [Header("Client")] 
    public GameObject clientPanel;
    public Text textJoinCodeIn;
    public Button joinButton;

    [Header("Other")] 
    public Text middleText;

    public Action<string> OnHostStarted;
    public Action OnClientStarted;
    
    public async void StartHost()
    {
        DisableUI("Creating match...");
        var result = await MultiplayerManager.Instance.StartHost();

        if (!result.Result)
        {
            Debug.LogError(result.Error);
            
            PopupUI.Instance.ShowPopUp("Error", result.Error, "Close");
            ResetUI();
            return;
        }
        
        OnHostStarted?.Invoke(result.JoinCode);
    }

    public async void JoinClient()
    {
        DisableUI("Joining match...");
        var result = await MultiplayerManager.Instance.JoinClient(textJoinCodeIn.text);

        if (!result.Result)
        {
            Debug.LogError(result.Error);
            
            PopupUI.Instance.ShowPopUp("Error", result.Error, "Close");
            ResetUI();
            return;
        }
        
        OnClientStarted?.Invoke();
    }

    private void DisableUI(string displayText)
    {
        hostPanel.SetActive(false);
        clientPanel.SetActive(false);
        hostButton.enabled = false;
        joinButton.enabled = false;
        
        middleText.text = displayText;
    }
    
    private void ResetUI()
    {
        hostPanel.SetActive(true);
        clientPanel.SetActive(true);
        hostButton.enabled = true;
        joinButton.enabled = true;
        
        middleText.text = "Or...";
    }
}
