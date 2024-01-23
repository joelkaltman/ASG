using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Database;

public static class AuthManager
{
    public static DependencyStatus dependencyStatus;
    static FirebaseAuth auth;    
    static FirebaseUser User;
    static DatabaseReference dbReference;

    public struct Result
    {
        public bool valid;
        public string error;

        public static Result Valid()
        {
            return new Result() { valid = true };
        }

        public static Result Error(string message)
        {
            return new Result() { error = message };
        }
    }

    public static async Task<Result> Initialize()
    {
        var initTask = FirebaseApp.CheckAndFixDependenciesAsync();
        
        await Task.Run(() => initTask);
        
        if (dependencyStatus == DependencyStatus.Available)
        {
            auth = FirebaseAuth.DefaultInstance;
            dbReference = FirebaseDatabase.DefaultInstance.RootReference;
            return Result.Valid();
        }

        return Result.Error("Service Unreachable");
    }

    public static void Logout()
    {
        auth.SignOut();
    }

    public static async Task<Result> Login(string email, string password)
    {
        //Call the Firebase auth signin function passing the email and password
        var loginTask = auth.SignInWithEmailAndPasswordAsync(email, password);
        //Wait until the task completes
        await Task.Run(() => loginTask);

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
            return Result.Error(message);
        }

        User = loginTask.Result.User;
        return Result.Valid();
    }

    public static async Task<Result> Register(string email, string password, string username)
    {
        var registerTask = auth.CreateUserWithEmailAndPasswordAsync(email, password);
        await Task.Run(() => registerTask);

        if (registerTask.Exception != null)
        {
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
            return Result.Error(message);
        }
        User = registerTask.Result.User;

        if (User != null)
        {
            await UpdateUserProfile(username);
            await UpdateUserData(0, 0);
        }

        return Result.Valid();
    }

    private static async Task UpdateUserProfile(string username)
    {
        UserProfile profile = new UserProfile() { DisplayName = username};

        var userTask = User.UpdateUserProfileAsync(profile);
        
        await Task.Run(() => userTask);
        
        if (userTask.Exception != null)
            Debug.Log($"Exception: {userTask.Exception}");
    }
    
    public static async Task UpdateUserData(int kills, int caps)
    {
        await WriteToDb("username", User.DisplayName);
        await WriteToDb("kills", kills);
        await WriteToDb("caps", caps);
    }

    private static async Task WriteToDb(string field, object value)
    {
        var userEntry = dbReference.Child("users").Child(User.UserId);
        
        var dbTask = userEntry.Child(field).SetValueAsync(value);
        
        await Task.Run(() => dbTask);

        if (dbTask.Exception != null)
            Debug.Log($"Exception: {dbTask.Exception}");
    }

    private static async Task GetUserData()
    {
        var username = await ReadDb("username");
        var kills = await ReadDb("kills");
    }

    private static async Task<object> ReadDb(string field)
    {
        var userEntry = dbReference.Child("users").Child(User.UserId);
        
        var dbTask = userEntry.Child(field).GetValueAsync();
        
        await Task.Run(() => dbTask);
        
        if (dbTask.Exception != null)
            Debug.Log($"Exception: {dbTask.Exception}");

        return dbTask.Result;
    }

    public static async Task GetScoreboard()
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
