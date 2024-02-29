using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PowerUpsManager : MonoBehaviour 
{
	public float spawnTime;

	float elapsedTimeSpawn;

	public GameObject spawnPointsContainer;
	private List<Transform> spawnPoints;
	private GameObject currentCap;
	private Transform lastCapSpawnPoint;
	
	void Start()
	{
		Transform[] transforms = spawnPointsContainer.GetComponentsInChildren<Transform> ();
		spawnPoints = new List<Transform> ();
		spawnPoints.AddRange (transforms);
		
		MultiplayerManager.Instance.OnGameReady += OnGameReady;
	}
	
	void OnGameReady()
	{
		if (!MultiplayerManager.Instance.IsHostReady)
		{
			Destroy(this);
			return;
		}
	}
	
	void Update ()
	{
		if (!MultiplayerManager.Instance.IsHostReady)
			return;
		
		elapsedTimeSpawn += Time.deltaTime;

		if (elapsedTimeSpawn >= spawnTime) {
			elapsedTimeSpawn = 0;
			SpawnPowerUp ();
		}

		if (currentCap == null) {
			SpawnCap ();
		}
	}

	void SpawnPowerUp()
	{
		return;
		int random1 = Random.Range (0, spawnPoints.Count);
		int random2 = Random.Range (0, GameData.Instance.powerUps.Count);

		/*bool found = false;
		int tries = 0;
		while (!found) {
			if (spawnPoints [random1].childCount == 0) {
				found = true;
			} else {
				random1 = Random.Range (0, spawnPoints.Count);
				tries++;
			}

			if (tries > 15) {
				found = true;
			}
		}

		GameObject.Instantiate (GameData.Instance.powerUps[random2], spawnPoints[random1]);*/

		for (int i = 0; i < transform.childCount; i++) {
			if (spawnPoints [random1].childCount == 0) {
				GameObject.Instantiate (GameData.Instance.powerUps [random2], spawnPoints [random1]);
				break;
			} else {
				random1 = Random.Range (0, spawnPoints.Count);
			}
		}

	}

	void SpawnCap()
	{
		int maxIterations = 1000;
		while (maxIterations > 0)
		{
			maxIterations--;
			
			int random = Random.Range (0, spawnPoints.Count);
			bool isFree = spawnPoints[random].childCount == 0;
			bool wasNotLast = lastCapSpawnPoint == null || lastCapSpawnPoint != spawnPoints[random];
			
			if (isFree && wasNotLast) {
				lastCapSpawnPoint = spawnPoints [random];
				currentCap = Instantiate (GameData.Instance.cap, spawnPoints [random]);
				currentCap.GetComponent<NetworkObject>()?.Spawn(true);
				break;
			}
		}
	}

	public Vector3 GetCapPosition()
	{
		if (currentCap != null) {
			return currentCap.transform.position;
		} else {
			return new Vector3 ();
		}
	}
}
