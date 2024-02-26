using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class EnemiesManager : MonoBehaviour {

	public static EnemiesManager Instance;
	public event Action onWaveChange;

	public List<Wave> waves;

	private List<Transform> spawnPoints;
	private List<GameObject> enemiesInstances;
	private float spawnTime;
	private float elapsedTimeWave;
	private float elapsedTimeSpawn;
	private int index;
	public int wave;

	public int waveDuration;

	private List<GameObject> players = new ();

	public void ResetEvents()
	{
		onWaveChange = null;
	}

	void Awake(){
		Instance = this;

		Transform[] transforms = this.GetComponentsInChildren<Transform> ();
		spawnPoints = new List<Transform> ();
		spawnPoints.AddRange (transforms);

		enemiesInstances = new List<GameObject> ();
		index = -1;
		wave = 0;
		spawnTime = 6;

		MultiplayerManager.Instance.OnGameReady += OnGameReady;
	}

	void OnGameReady()
	{
		players = MultiplayerManager.Instance.GetPlayers();
	}

	void OnEnable(){
		this.changeWave ();
	}

	void Update()
	{
		return;
		if (!MultiplayerManager.Instance.IsHostReady)
			return;
		
		bool anyPlayerAlive = players.Any(x => !x.GetComponent<PlayerStats>().IsDead);
		
		if (anyPlayerAlive && enemiesInstances.Count < 250) {
			elapsedTimeSpawn += Time.deltaTime;
			elapsedTimeWave += Time.deltaTime;
		}

		if (elapsedTimeSpawn >= spawnTime) {
			elapsedTimeSpawn = 0;
			SpawnEnemy (false);
		}

		if (elapsedTimeWave > waveDuration) {
			changeWave ();
		}
	}

	private void KillAllEnemies(){
		for (int i = 0; i < this.enemiesInstances.Count; i++) {
			EnemyStats stats = this.enemiesInstances [i].GetComponent<EnemyStats> ();
			if (stats != null) {
				stats.RecieveDamage (9999999, true, false);
			}
		}
	}

	private void SpawnEnemy(bool isBoss){
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
		enemiesInstances.Add(enemyInstance);
	}

	public void DestroyEnemy(GameObject enemy){
		GameObject enemyInstance = enemiesInstances.Find (obj => GameObject.ReferenceEquals(obj, enemy));
		enemiesInstances.Remove (enemy);
		Destroy (enemyInstance);
	}

	public List<GameObject> getInstantiatedEnemies(){
		return this.enemiesInstances;
	}

	public int getWaveDuration(){
		return this.waveDuration;
	}

	public GameObject GetClosestEnemyTo(Vector3 point){
		GameObject selected = null;
		float closest = 99999;
		for (int i = 0; i < this.enemiesInstances.Count; i++) {
			float distance = Vector3.Distance (this.enemiesInstances [i].transform.position, point);
			EnemyStats stats = this.enemiesInstances [i].GetComponent<EnemyStats> ();
			if(distance < closest && stats.life > 0){
				closest = distance;
				selected = this.enemiesInstances [i];
			}
		}
		return selected;
	}

	void changeWave(){
		index++;
		wave++;
		if (index >= waves.Count) {
			index--;
			spawnTime *= 0.5f;
			waveDuration += 10;
		} else {
			spawnTime = this.waves [index].frecuencySpawn;
			waveDuration = this.waves [index].durationSeconds;
		}

		elapsedTimeSpawn = 0;
		elapsedTimeWave = 0;

		for (int i = 0; i < this.waves [index].bossesToKill; i++) {
			SpawnEnemy(true);
		}

		onWaveChange?.Invoke ();
	}
}
