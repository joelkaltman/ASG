using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class WavesManager : NetworkBehaviour
{
    [HideInInspector] public NetworkVariable<int> Wave = new(-1);
    [HideInInspector] public NetworkVariable<int> WaveDuration = new();
    
    [HideInInspector] public NetworkVariable<int> Seconds = new();
    [HideInInspector] public NetworkVariable<int> Minutes = new();

    [SerializeField] private List<Wave> waves;

    private Wave additionalWave;
    
    [HideInInspector] public float SpawnTime;
    [HideInInspector] public float ElapsedTimeWave;

    private float elapsedTime;
    
    void Awake()
    {
        SpawnTime = 6;
    }

    private void Start()
    {
        MultiplayerManager.Instance.RegisterWaveManager(this);
    }

    void Update()
    {
        if(!MultiplayerManager.Instance.IsHostReady)
            return;
        
        bool anyPlayerAlive = MultiplayerManager.Instance.Players.Any(x => !x.GetComponent<PlayerStats>().IsDead);
		
        if (anyPlayerAlive) {
            ElapsedTimeWave += Time.deltaTime;
        }
        
        if (ElapsedTimeWave > WaveDuration.Value) 
        {
            ChangeWave ();
        }
        
        TakeTime();
    }
    
    void ChangeWave()
    {
        Wave.Value++;
        if (Wave.Value >= waves.Count) 
        {
            SpawnTime *= 0.5f;
            WaveDuration.Value += 10;
        } 
        else 
        {
            SpawnTime = waves [Wave.Value].frecuencySpawn;
            WaveDuration.Value = waves [Wave.Value].durationSeconds;
        }
        ElapsedTimeWave = 0;
        
        Minutes.Value = (int)Mathf.Floor (WaveDuration.Value / 60);
        Seconds.Value = Mathf.RoundToInt(WaveDuration.Value % 60);
    }

    public Wave CurrentWave()
    {
        return Wave.Value < waves.Count ? waves[Wave.Value] : waves.Last();
    }

    void TakeTime()
    {
        elapsedTime += Time.deltaTime;

        if (elapsedTime > 1)
        {
            elapsedTime -= 1;
            Seconds.Value--;
        }

        if (Seconds.Value < 0)
        {
            Seconds.Value = 59;
            Minutes.Value--;
        }
    }
}
