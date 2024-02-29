using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemiesManager : MonoBehaviour {

	public static EnemiesManager Instance;

	public GameObject spawnPointsContainer;
	private List<Transform> spawnPoints;
	public List<GameObject> EnemiesInstances { get; private set; }
	
	private List<GameObject> players = new ();

	private float elapsedTimeSpawn;

	private WavesManager wavesManager;

	void Start(){
		Instance = this;

		Transform[] transforms = spawnPointsContainer.GetComponentsInChildren<Transform> ();
		spawnPoints = new List<Transform> ();
		spawnPoints.AddRange (transforms);

		EnemiesInstances = new List<GameObject> ();

		MultiplayerManager.Instance.OnGameReady += OnGameReady;
	}

	void OnGameReady()
	{
		if (!MultiplayerManager.Instance.IsHostReady)
		{
			Destroy(this);
			return;
		}

		players = MultiplayerManager.Instance.Players;
		wavesManager = MultiplayerManager.Instance.WavesManager;
		wavesManager.Wave.OnValueChanged += OnChangeWave;
	}

	void Update()
	{
		if (!MultiplayerManager.Instance.IsHostReady)
			return;
		
		bool anyPlayerAlive = players.Any(x => !x.GetComponent<PlayerStats>().IsDead);
		
		if (anyPlayerAlive && EnemiesInstances.Count < 250) {
			elapsedTimeSpawn += Time.deltaTime;
		}

		if (elapsedTimeSpawn >= wavesManager.SpawnTime) {
			elapsedTimeSpawn = 0;
			SpawnEnemy (false);
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

		var listEnemies = isBoss ? wavesManager.CurrentWave().bosses : wavesManager.CurrentWave().enemies;
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

	void OnChangeWave(int previousWave, int currentWave)
	{
		for (int i = 0; i < wavesManager.CurrentWave().bossesToKill; i++) {
			SpawnEnemy(true);
		}
	}
}
