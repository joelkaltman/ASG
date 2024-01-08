using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpsManager : MonoBehaviour {

	[HideInInspector] public static PowerUpsManager Instance;

	public float spawnTime;

	float elapsedTimeSpawn;

	private List<Transform> spawnPoints;
	private GameObject currentCap;
	private Transform lastCapSpawnPoint;

	void Awake()
	{
		Instance = this;
	}

	void Start(){
		Transform[] transforms = this.GetComponentsInChildren<Transform> ();
		spawnPoints = new List<Transform> ();
		spawnPoints.AddRange (transforms);
	}

	void Update () {
		elapsedTimeSpawn += Time.deltaTime;

		if (elapsedTimeSpawn >= spawnTime) {
			elapsedTimeSpawn = 0;
			SpawnPowerUp ();
		}

		if (currentCap == null) {
			this.SpawnCap ();
		}
	}

	void SpawnPowerUp()
	{
		int random1 = Random.Range (0, this.spawnPoints.Count);
		int random2 = Random.Range (0, GameData.Instance.powerUps.Count);

		/*bool found = false;
		int tries = 0;
		while (!found) {
			if (this.spawnPoints [random1].childCount == 0) {
				found = true;
			} else {
				random1 = Random.Range (0, this.spawnPoints.Count);
				tries++;
			}

			if (tries > 15) {
				found = true;
			}
		}

		GameObject.Instantiate (GameData.Instance.powerUps[random2], this.spawnPoints[random1]);*/

		for (int i = 0; i < this.transform.childCount; i++) {
			if (this.spawnPoints [random1].childCount == 0) {
				GameObject.Instantiate (GameData.Instance.powerUps [random2], this.spawnPoints [random1]);
				break;
			} else {
				random1 = Random.Range (0, this.spawnPoints.Count);
			}
		}

	}

	void SpawnCap()
	{
		int random1 = Random.Range (0, this.spawnPoints.Count);

		/*bool found = false;
		int tries = 0;
		while (!found) {
			if (this.spawnPoints [random1].childCount == 0 &&
				(lastCapSpawnPoint == null || lastCapSpawnPoint != this.spawnPoints [random1])) {
				found = true;
			} else {
				random1 = Random.Range (0, this.spawnPoints.Count);
				tries++;
			}

			if (tries > 15) {
				found = true;
			}
		}

		lastCapSpawnPoint = this.spawnPoints [random1];
		this.currentCap = GameObject.Instantiate (GameData.Instance.cap, this.spawnPoints [random1]);*/


		for (int i = 0; i < this.transform.childCount; i++) {
			if (this.spawnPoints [random1].childCount == 0 && (lastCapSpawnPoint == null || lastCapSpawnPoint != this.spawnPoints [random1])) {
				lastCapSpawnPoint = this.spawnPoints [random1];
				this.currentCap = GameObject.Instantiate (GameData.Instance.cap, this.spawnPoints [random1]);
				break;
			} else {
				random1 = Random.Range (0, this.spawnPoints.Count);
			}
		}
	}

	public Vector3 getCapPosition()
	{
		if (this.currentCap != null) {
			return this.currentCap.transform.position;
		} else {
			return new Vector3 ();
		}
	}
}
