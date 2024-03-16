using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameData : MonoBehaviour
{
    public static GameData Instance;
    
    public bool isOnline;
    public string directJoinCode;
    public bool JoinWithDirectCode => !string.IsNullOrEmpty(directJoinCode);

    public GameObject cap;
    public List<GunData> guns;
    public List<GameObject> powerUps;
    
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
