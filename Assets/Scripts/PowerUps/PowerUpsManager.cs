using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PowerUpsManager : MonoBehaviour 
{
	public float spawnTime;

	float elapsedTimeSpawn;

	public GameObject spawnPointsContainer;
	private List<(Transform, GameObject)> spawnPoints;
	private GameObject currentCap;
	private int lastCapSpawnPoint;
	
	void Start()
	{
		Transform[] transforms = spawnPointsContainer.GetComponentsInChildren<Transform> ();
		spawnPoints = new ();

		foreach (var trans in transforms)
			spawnPoints.Add((trans, null));
		
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

		if (elapsedTimeSpawn >= spawnTime) 
		{
			elapsedTimeSpawn = 0;
			SpawnPowerUp ();
		}

		if (currentCap == null) 
		{
			SpawnCap ();
		}
	}

	void SpawnPowerUp()
	{
		int it = 1000;
		while (it > 0)
		{
			int random = Random.Range (0, spawnPoints.Count);
			bool isFree = spawnPoints[random].Item2 == null;
			
			if (isFree) 
			{
				int randomItem = Random.Range (0, GameData.Instance.powerUps.Count);
				var item = Instantiate (GameData.Instance.powerUps [randomItem], spawnPoints [random].Item1);
				item.GetComponent<NetworkObject>()?.Spawn(true);
				spawnPoints[random] = (spawnPoints[random].Item1, item);
				break;
			}
			it--;

		}

	}

	void SpawnCap()
	{
		int it = 1000;
		while (it > 0)
		{
			int random = Random.Range (0, spawnPoints.Count);
			bool isFree = spawnPoints[random].Item2 == null;
			bool wasNotLast = random != lastCapSpawnPoint;
			
			if (isFree && wasNotLast) 
			{
				lastCapSpawnPoint = random;
				currentCap = Instantiate (GameData.Instance.cap, spawnPoints [random].Item1);
				currentCap.GetComponent<NetworkObject>()?.Spawn(true);
				spawnPoints[random] = (spawnPoints[random].Item1, currentCap);
				break;
			}
			it--;
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
