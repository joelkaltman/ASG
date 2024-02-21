using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class UserManager
{
    private static UserManager userManager;
    public static UserManager Instance() => userManager ??= new UserManager();
    
    public AuthManager.UserData UserData { get; private set; }
    
    public bool Initialized { get; private set; }
    
    public event Action OnCapCountChange;

    public async Task Initialize()
    {
        UserData = await AuthManager.GetUserData();
        Initialized = true;
    }
    
    public void ResetEvents()
    {
        OnCapCountChange = null;
    }

    public void Clean()
    {
        UserData = null;
        Initialized = false;
    }
    
    public async void SaveUserData()
    {
        await AuthManager.SaveUserData(UserData);
    }

    public async void SaveKills()
    {
        await AuthManager.WriteToDb("kills", UserData.maxKills);
    }
	
    public async void SaveCaps()
    {
        await AuthManager.WriteToDb("caps", UserData.caps);
    }
	
    public async void SaveGuns()
    {
        await AuthManager.WriteToDb("guns", UserData.guns);
    }
    
    public void AddCap()
    {
        UserData.caps++;
        SaveCaps();
        
        OnCapCountChange?.Invoke ();
    }

    public void PurchaseGun(GunData gun)
    {
        UserData.guns.Add(gun.Id);
        SaveGuns();
		
        UserData.caps -= gun.Price;
        SaveCaps();
		
        OnCapCountChange?.Invoke ();
    }
}
