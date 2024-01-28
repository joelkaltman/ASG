using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginUI : MonoBehaviour
{
    public Button loginButton;
    public Button registerButton;

    public Text usernameText;
    public Text passwordText;

    public static AuthManager.UserData userDataLogin;

    private bool initialized;
    
    void Start()
    {
        userDataLogin = null;
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

    public async void Login()
    {
        await InitializeAuth();
        
        string email = usernameText.text + "@gmail.com";
        string password = passwordText.text;
        var result = await AuthManager.Login(email, password);

        if (!result.valid)
        {
            Debug.LogError(result.error);
            return;
        }

        userDataLogin = await AuthManager.GetUserData();
        
        SceneManager.LoadScene("Main", LoadSceneMode.Single);
    }
    
    public async void Register()
    {
        await InitializeAuth();
        
        string username = usernameText.text;
        string email = usernameText.text + "@gmail.com";
        string password = passwordText.text;
        var result = await AuthManager.Register(email, password, username, 200, new List<int> {0});
        
        if (!result.valid)
        {
            Debug.LogError(result.error);
            return;
        }
        
        SceneManager.LoadScene("Main", LoadSceneMode.Single);
    }
}
