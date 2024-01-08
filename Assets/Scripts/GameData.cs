using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour {

	public static GameData Instance = null;

	public bool isMobile;

	public GameObject cap;
	public List<GunData> guns;
	public List<GameObject> enemies;
	public List<GameObject> powerUps;

	void Awake() {
		if (Instance != null && Instance != this) {
			Destroy (this.gameObject);
			return;
		}else{
			Instance = this;
			DontDestroyOnLoad (this.gameObject);
		}
	}
}
