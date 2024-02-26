using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{
    public static GameData Instance;

    public GameDataScriptable gameDataScriptable;
    
    public bool isOnline;

    public GameObject cap => gameDataScriptable.cap;
    public List<GunData> guns => gameDataScriptable.guns;
    public List<GameObject> powerUps => gameDataScriptable.powerUps;
    
    public void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this);
    }
}
