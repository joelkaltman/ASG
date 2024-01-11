using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class EnemiesManager : MonoBehaviour {

	[HideInInspector]public static EnemiesManager Instance;
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
	}

	void OnEnable(){
		this.changeWave ();
	}

	void Update(){
		if (PlayerStats.Instance.life > 0 && enemiesInstances.Count < 250) {
			elapsedTimeSpawn += Time.deltaTime;
			elapsedTimeWave += Time.deltaTime;
		}

		if (elapsedTimeSpawn >= spawnTime) {
			elapsedTimeSpawn = 0;
			this.SpawnEnemy ();
		}

		if (elapsedTimeWave > waveDuration) {
			this.changeWave ();
		}
	}

	private void killAllEnemies(){
		for (int i = 0; i < this.enemiesInstances.Count; i++) {
			EnemyStats stats = this.enemiesInstances [i].GetComponent<EnemyStats> ();
			if (stats != null) {
				stats.RecieveDamage (9999999, true, false);
			}
		}
	}

	public void SpawnEnemy(){
		Vector3 pos = new Vector3 ();

		bool posFound = false;
		while(!posFound){
			int random = Random.Range (0, spawnPoints.Count);
			pos = spawnPoints [random].position;

			if(Vector3.Distance(pos, PlayerStats.Instance.getPlayer().transform.position) >= 10){
				posFound = true;
			}
		}
			
		int r = Random.Range (0, this.waves [index].enemies.Count);

		GameObject enemyInstance = Instantiate(this.waves [index].enemies[r], pos, Quaternion.identity);
		enemiesInstances.Add(enemyInstance);
	}

	public void SpawnBoss(){
		Vector3 pos = new Vector3 ();

		bool posFound = false;
		while(!posFound){
			int random = Random.Range (0, spawnPoints.Count);
			pos = spawnPoints [random].position;

			if(Vector3.Distance(pos, PlayerStats.Instance.getPlayer().transform.position) >= 10){
				posFound = true;
			}
		}

		int r = Random.Range (0, this.waves [index].bosses.Count);

		GameObject enemyInstance = Instantiate(this.waves [index].bosses[r], pos, Quaternion.identity);
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
			this.SpawnBoss ();
		}

		onWaveChange?.Invoke ();
	}
}
