using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Waves/Wave")]
public class Wave : ScriptableObject {

	public int durationSeconds;
	public List<GameObject> bosses;
	public int bossesToKill;
	public List<GameObject> enemies;
	public float frecuencySpawn;
}
