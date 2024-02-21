using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName="GameData")]
public class GameDataScriptable : ScriptableObject
{
	public bool isMobile;

	public GameObject cap;
	public List<GunData> guns;
	public List<GameObject> powerUps;
}
