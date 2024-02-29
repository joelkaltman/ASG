using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class WavesManager : NetworkBehaviour
{
    public NetworkVariable<int> Wave = new();
    public NetworkVariable<int> WaveDuration = new();
    
    public NetworkVariable<int> Seconds = new();
    public NetworkVariable<int> Minutes = new();

    public List<Wave> waves;
    
    public float SpawnTime;
    public float ElapsedTimeWave;
    private int index;

    private float elapsedTime;
    
    void Awake()
    {
        index = -1;
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
        index++;
        Wave.Value++;
        if (index >= waves.Count) {
            index--;
            SpawnTime *= 0.5f;
            WaveDuration.Value += 10;
        } else {
            SpawnTime = waves [index].frecuencySpawn;
            WaveDuration.Value = waves [index].durationSeconds;
        }
        ElapsedTimeWave = 0;
        
        Minutes.Value = (int)Mathf.Floor (WaveDuration.Value / 60);
        Seconds.Value = Mathf.RoundToInt(WaveDuration.Value % 60);
    }

    public Wave CurrentWave()
    {
        return waves[index];
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
