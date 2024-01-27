using System.Collections;
using System.Collections.Generic;
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
    
    void Start()
    {
        AuthManager.Initialize().ContinueWith((task) =>
        {
            if (!task.Result.valid)
            {
                Debug.LogError(task.Result.error);
            }
        });
        
        loginButton.onClick.AddListener(Login);
        registerButton.onClick.AddListener(Register);
    }

    private async void Login()
    {
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
    
    private async void Register()
    {
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
