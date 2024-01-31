using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = System.Random;

public class LoginUI : MonoBehaviour
{
    public Text usernameText;
    public Text passwordText;

    public GameObject loadingText;
    public GameObject inputPanel;

    public static AuthManager.UserData userDataLogin;

    private bool initialized;
    private static bool usedAutomaticLogin;

    private const string emailSufix = "@asg.com";
    
    void Start()
    {
        userDataLogin = null;
        
        if (usedAutomaticLogin)
        {
            // loged out
            SavePlayerPrefs("", "");
            return;
        }
        
        TryAutomaticLogic();
    }

    private async Task CreateFakeUsers()
    {
        List<string> users = new List<string>()
        {
            "Actorgael",
            "BizarreMedium",
            "Copebr",
            "Dominte",
            "Farrenbe",
            "FraserFootball",
            "Giblup",
            "GodzillaSra",
            "GrabsStar",
            "Griffonic",
            "Guantoci",
            "InspiringIce",
            "Jeanac",
            "Kinost",
            "LifeLegend",
            "LiveStronger",
            "MaidKrypto",
            "MisterHunter",
            "Murphydr",
            "Neareg",
            "Poddapp",
            "Remoldpa",
            "Sarahanch",
            "ShadesGold",
            "Stablacco",
            "Staceyma",
            "Trickfi",
            "Xpotri",
            "ZinPrecise"
        };

        await InitializeAuth();
        
        Random random = new Random();
        foreach (var user in users)
        {
            if(user.Length < 6 || user.Contains(" "))
                continue;
            
            var result = await AuthManager.Register(user + emailSufix, user, user, 100, new List<int> {0});
            
            if (!result.valid)
            {
                Debug.LogError("Error: " + result.error);
                return;
            }
            await AuthManager.WriteToDb("kills", random.Next(5, 250));
            //uthManager.Logout();
            
            Debug.Log("FINISHED " + user);
            await Task.Delay(100);
        }
        
        
    }

    private async void TryAutomaticLogic()
    {
        inputPanel.SetActive(false);
        loadingText.SetActive(true);
        
        if (!usedAutomaticLogin && GetPlayerPrefs(out string email, out string password))
        {
            usedAutomaticLogin = true;
            if(await Login(email, password))
                return;
        }
        
        inputPanel.SetActive(true);
        loadingText.SetActive(false);
    }

    private bool GetPlayerPrefs(out string email, out string password)
    {
        if (!PlayerPrefs.HasKey("email") || !PlayerPrefs.HasKey("password"))
        {
            email = null;
            password = null;
            return false;
        }

        email = PlayerPrefs.GetString("email");
        password = PlayerPrefs.GetString("password");
        return !string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password);
    }
    
    private void SavePlayerPrefs(string email, string password)
    {
        PlayerPrefs.SetString("email", email);
        PlayerPrefs.SetString("password", password);
    }

    private async Task InitializeAuth()
    {
        if (initialized)
            return;
        
        var result = await AuthManager.Initialize();
        initialized = result.valid;
        if (!initialized)
        {
            Debug.LogError(result.error);
        }
    }

    public void LoginButton()
    {
        if (!ValidateFields())
            return;
        
        string email = usernameText.text + emailSufix;
        string password = passwordText.text;
        
        Login(email, password);
    }

    private async Task<bool> Login(string email, string password)
    {
        await InitializeAuth();

        var result = await AuthManager.Login(email, password);

        if (!result.valid)
        {
            PopupUI.Instance.ShowPopUp("Error", result.error, "Ok");
            return false;
        }

        SavePlayerPrefs(email, password);
        EnterGame();
        return true;
    }

    public void RegisterButton()
    {
        if (!ValidateFields())
            return;
        
        string username = usernameText.text;
        string email = usernameText.text + emailSufix;
        string password = passwordText.text;
        
        Register(username, email, password);
    }

    public async Task Register(string username, string email, string password)
    {
        await InitializeAuth();
        
        var result = await AuthManager.Register(email, password, username, 100, new List<int> {0});
        
        if (!result.valid)
        {
            PopupUI.Instance.ShowPopUp("Error", result.error, "Ok");
            return;
        }
        
        SavePlayerPrefs(email, password);
        EnterGame();
    }

    private async void EnterGame()
    {
        userDataLogin = await AuthManager.GetUserData();
        SceneManager.LoadScene("Main", LoadSceneMode.Single);
    }

    private bool ValidateFields()
    {
        if (usernameText.text.Length == 0)
        {
            PopupUI.Instance.ShowPopUp("Error", "Username is empty!", "Ok");
            return false;
        }

        if (usernameText.text.Length < 6)
        {
            PopupUI.Instance.ShowPopUp("Error", "Username is too short! Needs to have at least 6 characters.", "Ok");
            return false;
        }
        
        if (usernameText.text.Contains(" "))
        {
            PopupUI.Instance.ShowPopUp("Error", "Username cannot contain spaces!", "Ok");
            return false;
        }

        if (passwordText.text.Length == 0)
        {
            PopupUI.Instance.ShowPopUp("Error", "Password is empty!", "Ok");
            return false;
        }

        if(passwordText.text.Length < 6)
        {
            PopupUI.Instance.ShowPopUp("Error", "Password is too short! Needs to have at least 6 characters.", "Ok");
            return false;
        }
        
        if (passwordText.text.Contains(" "))
        {
            PopupUI.Instance.ShowPopUp("Error", "Password cannot contain spaces!", "Ok");
            return false;
        }

        return true;
    }
}
