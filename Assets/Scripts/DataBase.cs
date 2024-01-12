using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DataBase : MonoBehaviour {

	public static DataBase Instance;

	DataBaseEntry currentEntry;

	string path = "Assets/DataBase.txt";

	void Awake() {
		if (Instance == null) {
			Instance = this;
			DontDestroyOnLoad (this.gameObject);
		}
		currentEntry = new DataBaseEntry ();

		loadEntry ();
	}

	public void deleteAllData()
	{
		PlayerPrefs.DeleteAll ();
	}

	string convertToJSON(DataBaseEntry entry){
		return JsonUtility.ToJson (entry);
	}

	DataBaseEntry parseJSON(string json){
		return JsonUtility.FromJson<DataBaseEntry> (json);
	}

	void loadEntry(){
		string json = PlayerPrefs.GetString ("DataBaseJSON2", "");
		if (json == "") {
			currentEntry.caps = 200;
			currentEntry.lights = false;
			currentEntry.maxScores.Clear ();
			currentEntry.maxScoresNames.Clear ();
			currentEntry.gunIndices.Clear ();

			currentEntry.gunIndices.Add (0);
			for (int i = 10; i > 0; i--) {
				currentEntry.maxScores.Add (i * 10);
				currentEntry.maxScoresNames.Add ("ASG");
			}
		} else {
			this.currentEntry = this.parseJSON (json);
		}

		/*try	{
			StreamReader reader = new StreamReader(path);
			this.currentEntry = this.parseJSON (reader.ReadToEnd ());
		} catch(FileNotFoundException) {
			this.storeEntry ();
		}*/
	}

	void storeEntry(){
		string json = this.convertToJSON (this.currentEntry);
		PlayerPrefs.SetString ("DataBaseJSON2", json);
		PlayerPrefs.Save ();

		//StreamWriter writer = new StreamWriter(path, true);
		//writer.Write(json);
		//writer.Close();
	}

	public void SaveCaps(int caps){
		this.currentEntry.caps = caps;
		this.storeEntry ();
	}

	public int LoadCaps(){
		this.loadEntry ();
		return currentEntry.caps;
	}

	public void SaveNewGunIndex(int index)
	{
		this.currentEntry.gunIndices.Add(index);
		this.storeEntry ();
	}

	public void SaveGunIndices(List<int> indices)
	{
		this.currentEntry.gunIndices = indices;
		this.storeEntry ();
	}

	public List<int> LoadGunIndices()
	{
		this.loadEntry ();
		return this.currentEntry.gunIndices;
	}

	public bool loadLights()
	{
		this.loadEntry ();
		return this.currentEntry.lights;
	}

	public void saveLights(bool lights)
	{
		this.currentEntry.lights = lights;
		this.storeEntry();
	}

	public List<int> LoadMaxScores()
	{
		this.loadEntry ();
		return this.currentEntry.maxScores;
	}

	public List<string> LoadMaxScoresNames()
	{
		this.loadEntry ();
		return this.currentEntry.maxScoresNames;
	}

	public bool isMaxScore(int score)
	{
		for (int i = 0; i < this.currentEntry.maxScores.Count; i++) {
			if (score >= this.currentEntry.maxScores [i]) {
				return true;
			}
		}
		return false;
	}

	public int SaveNewMaxScore(int score, string name)
	{
		int rankPos = -1;
		List<int> newMaxScores = new List<int> ();
		List<string> newMaxScoresNames = new List<string> ();
		for (int i = 0; i < this.currentEntry.maxScores.Count; i++) {
			if (score >= this.currentEntry.maxScores [i] && rankPos == -1) {
				newMaxScores.Add (score);
				newMaxScoresNames.Add (name);
				rankPos = i;
			}
			if (newMaxScores.Count < 10) {
				newMaxScores.Add (this.currentEntry.maxScores [i]);
				newMaxScoresNames.Add (this.currentEntry.maxScoresNames [i]);
			}
		}

		this.currentEntry.maxScores = newMaxScores;
		this.currentEntry.maxScoresNames = newMaxScoresNames;

		this.storeEntry ();

		return rankPos;
	}

	public bool EntryHasGun(int gunIndex){
		for (int i = 0; i < currentEntry.gunIndices.Count; i++) {
			if (currentEntry.gunIndices [i] == gunIndex) {
				return true;
			}
		}
		return false;
	}
}
