using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using TMPro;

public class AuthManager : MonoBehaviour
{
    //Firebase variables
    [Header("Firebase")]
    public DependencyStatus dependencyStatus;
    public FirebaseAuth auth;    
    public FirebaseUser User;
    public DatabaseReference dbReference;

    void Awake()
    {
        //Check that all of the necessary dependencies for Firebase are present on the system
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                //If they are avalible Initialize Firebase
                InitializeFirebase();
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
        });
    }

    private void InitializeFirebase()
    {
        Debug.Log("Setting up Firebase Auth");
        //Set the authentication instance object
        auth = FirebaseAuth.DefaultInstance;
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;
    }
    
    //Function for the login button
    public async void LoginButton()
    {
        //Call the login coroutine passing the email and password
        //StartCoroutine(Login(emailLoginField.text, passwordLoginField.text));
        await Login("tami@gmail.com", "asdasd");
    }
    //Function for the register button
    public async void RegisterButton()
    {
        //Call the register coroutine passing the email, password, and username
        //StartCoroutine(Register(emailRegisterField.text, passwordRegisterField.text, usernameRegisterField.text));
        //StartCoroutine(Register("tami@gmail.com", "asdasd", "tami"));
        
        await UpdateUserData(76);
        //GetUserData();
        await GetScoreboard();
    }

    public void Logout()
    {
        auth.SignOut();
    }

    private async Task Login(string _email, string _password)
    {
        //Call the Firebase auth signin function passing the email and password
        var loginTask = auth.SignInWithEmailAndPasswordAsync(_email, _password);
        //Wait until the task completes
        await Task.Run(() => loginTask.IsCompleted);

        if (loginTask.Exception != null)
        {
            //If there are errors handle them
            Debug.LogWarning(message: $"Failed to register task with {loginTask.Exception}");
            FirebaseException firebaseEx = loginTask.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

            string message = "Login Failed!";
            switch (errorCode)
            {
                case AuthError.MissingEmail:
                    message = "Missing Email";
                    break;
                case AuthError.MissingPassword:
                    message = "Missing Password";
                    break;
                case AuthError.WrongPassword:
                    message = "Wrong Password";
                    break;
                case AuthError.InvalidEmail:
                    message = "Invalid Email";
                    break;
                case AuthError.UserNotFound:
                    message = "Account does not exist";
                    break;
            }
            Debug.Log($"Warning: {message}");
        }
        else
        {
            //User is now logged in
            //Now get the result
            User = loginTask.Result.User;
            Debug.LogFormat("User signed in successfully: {0} ({1})", User.DisplayName, User.Email);  
        }
    }

    private async Task Register(string email, string password, string username)
    {
        /*if (_username == "")
        {
            //If the username field is blank show a warning
            warningRegisterText.text = "Missing Username";
        }
        else if(passwordRegisterField.text != passwordRegisterVerifyField.text)
        {
            //If the password does not match show a warning
            warningRegisterText.text = "Password Does Not Match!";
        }
        else*/ 
        {
            //Call the Firebase auth signin function passing the email and password
            var registerTask = auth.CreateUserWithEmailAndPasswordAsync(email, password);
            //Wait until the task completes
            await Task.Run(() => registerTask.IsCompleted);

            if (registerTask.Exception != null)
            {
                //If there are errors handle them
                Debug.LogWarning(message: $"Failed to register task with {registerTask.Exception}");
                FirebaseException firebaseEx = registerTask.Exception.GetBaseException() as FirebaseException;
                AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

                string message = "Register Failed!";
                switch (errorCode)
                {
                    case AuthError.MissingEmail:
                        message = "Missing Email";
                        break;
                    case AuthError.MissingPassword:
                        message = "Missing Password";
                        break;
                    case AuthError.WeakPassword:
                        message = "Weak Password";
                        break;
                    case AuthError.EmailAlreadyInUse:
                        message = "Email Already In Use";
                        break;
                }
                //warningRegisterText.text = message;
                Debug.Log($"Warning: {message}");
            }
            else
            {
                //User has now been created
                //Now get the result
                User = registerTask.Result.User;

                if (User != null)
                {
                    await UpdateUserProfile(username);
                }
            }
        }
    }

    private async Task UpdateUserProfile(string username)
    {
        UserProfile profile = new UserProfile() { DisplayName = username};

        var userTask = User.UpdateUserProfileAsync(profile);
        
        await Task.Run(() => userTask);
        
        if (userTask.Exception != null)
            Debug.Log($"Exception: {userTask.Exception}");
    }

    private async Task UpdateUserData(int kills)
    {
        await WriteToDb("username", User.DisplayName);
        await WriteToDb("kills", kills);
    }

    private async Task WriteToDb(string field, object value)
    {
        var userEntry = dbReference.Child("users").Child(User.UserId);
        
        var dbTask = userEntry.Child(field).SetValueAsync(value);
        
        await Task.Run(() => dbTask);

        if (dbTask.Exception != null)
            Debug.Log($"Exception: {dbTask.Exception}");
    }

    private async Task GetUserData()
    {
        var username = await ReadDb("username");
        var kills = await ReadDb("kills");
    }

    private async Task<object> ReadDb(string field)
    {
        var userEntry = dbReference.Child("users").Child(User.UserId);
        
        var dbTask = userEntry.Child(field).GetValueAsync();
        
        await Task.Run(() => dbTask);
        
        if (dbTask.Exception != null)
            Debug.Log($"Exception: {dbTask.Exception}");

        return dbTask.Result;
    }

    private async Task GetScoreboard()
    {
        var dbTask = dbReference.Child("users").OrderByChild("kills").GetValueAsync();
        
        await Task.Run(() => dbTask);
        
        if (dbTask.Exception != null)
            Debug.Log($"Exception: {dbTask.Exception}");

        DataSnapshot dataSnapshot = dbTask.Result;

        foreach (var data in dataSnapshot.Children.Reverse())
        {
            var username = data.Child("username");
            var kills = data.Child("kills");
            Debug.Log($"User: {username} - Kills: {kills}");
        }
    }
}
