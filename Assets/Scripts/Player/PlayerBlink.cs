using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBlink : MonoBehaviour {

	public Texture normalTexture;
	public Texture blinkingTexture;

	// Use this for initialization
	void Start () {
		InvokeRepeating ("Blink", 0, 3);
		InvokeRepeating ("Blink", 0, 9.5f);
	}
	
	// Update is called once per frame
	void Update () {
	}

	public void Blink()
	{
		Material playerMat = this.GetComponentInChildren<Renderer> ().material;
		playerMat.mainTexture = blinkingTexture;
		Invoke ("Open", 0.1f);
	}

	public void Open()
	{
		Material playerMat = this.GetComponentInChildren<Renderer> ().material;
		playerMat.mainTexture = normalTexture;
	}
}
