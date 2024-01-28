using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginUI : MonoBehaviour
{
    public Text usernameText;
    public Text passwordText;

    public GameObject loadingText;
    public GameObject inputPanel;

    public static AuthManager.UserData userDataLogin;

    private bool initialized;
    private static bool usedAutomaticLogin;
    
    void Start()
    {
        userDataLogin = null;
        TryAutomaticLogic();
    }

    private async void TryAutomaticLogic()
    {
        inputPanel.SetActive(false);
        
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
        return true;
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
        string email = usernameText.text + "@gmail.com";
        string password = passwordText.text;
        
        Login(email, password);
    }

    private async Task<bool> Login(string email, string password)
    {
        await InitializeAuth();

        var result = await AuthManager.Login(email, password);

        if (!result.valid)
        {
            Debug.LogError(result.error);
            return false;
        }

        SavePlayerPrefs(email, password);
        EnterGame();
        return true;
    }

    public void RegisterButton()
    {
        string username = usernameText.text;
        string email = usernameText.text + "@gmail.com";
        string password = passwordText.text;
        
        Register(username, email, password);
    }

    public async void Register(string username, string email, string password)
    {
        await InitializeAuth();
        
        var result = await AuthManager.Register(email, password, username, 200, new List<int> {0});
        
        if (!result.valid)
        {
            Debug.LogError(result.error);
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
}
