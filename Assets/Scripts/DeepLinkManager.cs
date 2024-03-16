using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeepLinkManager : MonoBehaviour
{
    public static DeepLinkManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;                
            Application.deepLinkActivated += OnDeepLinkActivated;
            if (!string.IsNullOrEmpty(Application.absoluteURL))
            {
                OnDeepLinkActivated(Application.absoluteURL);
            }
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        OnDeepLinkActivated("asd?rtlh9p");
    }

    private async void OnDeepLinkActivated(string code)
    {
        string joinCode = code.Split('?')[1];

        bool loggedIn = UserManager.Instance().Initialized;
        if (!loggedIn)
            loggedIn = await UserManager.Instance().TryDirectLogin();

        if (!loggedIn)
        {
            PopupUI.Instance.ShowPopUp("Login Error", "Didn't manage to login. Please try entering your credentials.", "Ok");
            return;
        }

        GameData.Instance.isOnline = true;
        GameData.Instance.directJoinCode = joinCode;
        SceneManager.LoadScene("Game");
    }
}
