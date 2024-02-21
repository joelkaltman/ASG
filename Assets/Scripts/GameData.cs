using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{
    public static GameData Instance;

    public GameDataScriptable gameDataScriptable;
    
    public bool isMobile => gameDataScriptable.isMobile;

    public GameObject cap => gameDataScriptable.cap;
    public List<GunData> guns => gameDataScriptable.guns;
    public List<GameObject> powerUps => gameDataScriptable.powerUps;
    
    public void Start()
    {
        Instance = this;
        DontDestroyOnLoad(this);
    }
}
