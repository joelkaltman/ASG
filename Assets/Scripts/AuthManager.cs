using System;
using System.Collections.Generic;
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
        try
        {
            var checkDep = FirebaseApp.CheckDependenciesAsync();
            var check = await checkDep;
                
            var initTask = FirebaseApp.CheckAndFixDependenciesAsync();
            var initResult = await initTask;
            
            if (dependencyStatus == initResult)
            {
                auth = FirebaseAuth.DefaultInstance;
                dbReference = FirebaseDatabase.DefaultInstance.RootReference;
                return Result.Valid();
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
        

        return Result.Error("Service Unreachable");
    }

    public static void Logout()
    {
        auth.SignOut();
        auth = null;
        User = null;
        dbReference = null;
    }

    public static async Task<Result> Login(string email, string password)
    {
        var loginTask = auth.SignInWithEmailAndPasswordAsync(email, password);

        try
        {
            var loginResult = await loginTask;
            User = loginResult.User;
        }
        catch (Exception exception)
        {
            return Result.Error(exception.Message);
        }

        return Result.Valid();
    }

    public static async Task<Result> Register(string email, string password, string username, int caps, List<int> guns)
    {
        var registerTask = auth.CreateUserWithEmailAndPasswordAsync(email, password);
        try
        {
            var register = await registerTask;
            User = register.User;
        }
        catch (Exception exception)
        {
            return Result.Error(exception.Message);
        }

        if (User != null)
        {
            await UpdateUserProfile(username);
            await SaveUserData(new UserData() { username = username, caps = caps, guns = guns });
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
    
    public class UserData
    {
        public string username;
        public int maxKills;
        public int caps;
        public List<int> guns = new ();
    }
    
    public static async Task<bool> SaveUserData(UserData userData)
    {
        bool result = true;
        result &= await WriteToDb("username", userData.username);
        result &= await WriteToDb("kills", userData.maxKills);
        result &= await WriteToDb("caps", userData.caps);
        result &= await WriteToDb("guns", userData.guns);
        return result;
    }

    public static async Task<bool> WriteToDb(string field, object value)
    {
        var userEntry = dbReference.Child("users").Child(User.UserId);
        
        var dbTask = userEntry.Child(field).SetValueAsync(value);
        
        await Task.Run(() => dbTask);

        if (dbTask.Exception != null)
        {
            Debug.Log($"Exception: {dbTask.Exception}");
            return false;
        }

        Debug.Log($"{field} for user {User.DisplayName} has been updated!");
        return true;
    }

    public static async Task<UserData> GetUserData()
    {
        var username = await ReadDb<string>("username");
        var kills = await ReadDb<long>("kills");
        var caps = await ReadDb<long>("caps");
        var guns = await ReadDb<List<object>>("guns");
        
        return new UserData()
        {
            username = username,
            maxKills = Convert.ToInt32(kills),
            caps = Convert.ToInt32(caps),
            guns = guns.Select(Convert.ToInt32).ToList()
        };
    }

    private static async Task<T> ReadDb<T>(string field)
    {
        var userEntry = dbReference.Child("users").Child(User.UserId);
        
        var dbTask = userEntry.Child(field).GetValueAsync();
        
        await Task.Run(() => dbTask);
        
        if (dbTask.Exception != null)
            Debug.Log($"Exception: {dbTask.Exception}");

        return dbTask.Result.Value != null ? (T)dbTask.Result.Value : default;
    }

    public struct UserRank
    {
        public string username;
        public int maxKills;

        public UserRank(object username, object kills)
        {
            this.username = username != null ? (string)username : default;
            this.maxKills = kills != null ? Convert.ToInt32((long)kills) : default;
        }
    }
    
    public static async Task<List<UserRank>> GetScoreboard()
    {
        var dbTask = dbReference.Child("users").OrderByChild("kills").GetValueAsync();
        
        await Task.Run(() => dbTask);
        
        if (dbTask.Exception != null)
            Debug.Log($"Exception: {dbTask.Exception}");

        DataSnapshot dataSnapshot = dbTask.Result;

        List<UserRank> userRanks = new();
        foreach (var data in dataSnapshot.Children.Reverse())
        {
            var username = data.Child("username").Value;
            var kills = data.Child("kills").Value;
            
            userRanks.Add(new UserRank(username, kills));
        }

        return userRanks;
    }
}
