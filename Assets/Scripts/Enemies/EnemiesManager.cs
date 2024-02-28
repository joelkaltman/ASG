using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class EnemiesManager : NetworkBehaviour {

	public static EnemiesManager Instance;
	public List<Wave> waves;

	public GameObject spawnPointsContainer;
	private List<Transform> spawnPoints;
	public List<GameObject> EnemiesInstances { get; private set; }
	private float spawnTime;
	private float elapsedTimeWave;
	private float elapsedTimeSpawn;
	private int index;
	
	public NetworkVariable<int> Wave = new();
	public NetworkVariable<int> WaveDuration = new();

	private List<GameObject> players = new ();

	void Awake(){
		Instance = this;

		Transform[] transforms = spawnPointsContainer.GetComponentsInChildren<Transform> ();
		spawnPoints = new List<Transform> ();
		spawnPoints.AddRange (transforms);

		EnemiesInstances = new List<GameObject> ();
		index = -1;
		Wave.Value = 0;
		spawnTime = 6;

		MultiplayerManager.Instance.OnGameReady += OnGameReady;
		
	}

	void OnGameReady()
	{
		if (!MultiplayerManager.Instance.IsHostReady)
		{
			enabled = false;
			return;
		}

		players = MultiplayerManager.Instance.Players;
	}

	void OnEnable()
	{
		if (!MultiplayerManager.Instance.IsHostReady)
			return;
		
		ChangeWave ();
	}

	void Update()
	{
		if (!MultiplayerManager.Instance.IsHostReady)
			return;
		
		bool anyPlayerAlive = players.Any(x => !x.GetComponent<PlayerStats>().IsDead);
		
		if (anyPlayerAlive && EnemiesInstances.Count < 250) {
			elapsedTimeSpawn += Time.deltaTime;
			elapsedTimeWave += Time.deltaTime;
		}

		if (elapsedTimeSpawn >= spawnTime) {
			elapsedTimeSpawn = 0;
			SpawnEnemy (false);
		}

		if (elapsedTimeWave > WaveDuration.Value) {
			ChangeWave ();
		}
	}

	private void SpawnEnemy(bool isBoss)
	{
		Vector3 pos = new Vector3 ();

		var playersPos = players.Select(x => x.transform.position).ToList();
		
		bool posFound = false;
		while(!posFound){
			int random = Random.Range (0, spawnPoints.Count);
			pos = spawnPoints [random].position;

			if(playersPos.All(playerPos => Vector3.Distance(pos, playerPos) >= 10)){
				posFound = true;
			}
		}

		var listEnemies = isBoss ? waves[index].bosses : waves[index].enemies;
		int r = Random.Range (0, listEnemies.Count);

		GameObject enemyInstance = Instantiate(listEnemies[r], pos, Quaternion.identity);
		enemyInstance.GetComponent<NetworkObject>().Spawn();
		EnemiesInstances.Add(enemyInstance);
	}

	public void DestroyEnemy(GameObject enemy)
	{
		if (!MultiplayerManager.Instance.IsHostReady)
			return;
		
		GameObject enemyInstance = EnemiesInstances.Find (obj => GameObject.ReferenceEquals(obj, enemy));
		EnemiesInstances.Remove (enemy);
		Destroy (enemyInstance);
	}

	public GameObject GetClosestEnemyTo(Vector3 point)
	{
		GameObject selected = null;
		float closest = 99999;
		for (int i = 0; i < EnemiesInstances.Count; i++) {
			float distance = Vector3.Distance (EnemiesInstances [i].transform.position, point);
			EnemyStats stats = EnemiesInstances [i].GetComponent<EnemyStats> ();
			if(distance < closest && stats.life > 0){
				closest = distance;
				selected = EnemiesInstances [i];
			}
		}
		return selected;
	}

	void ChangeWave()
	{
		index++;
		Wave.Value++;
		if (index >= waves.Count) {
			index--;
			spawnTime *= 0.5f;
			WaveDuration.Value += 10;
		} else {
			spawnTime = waves [index].frecuencySpawn;
			WaveDuration.Value = waves [index].durationSeconds;
		}

		elapsedTimeSpawn = 0;
		elapsedTimeWave = 0;

		for (int i = 0; i < waves [index].bossesToKill; i++) {
			SpawnEnemy(true);
		}
	}
}
