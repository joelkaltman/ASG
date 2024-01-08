using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BounceText : MonoBehaviour {

	public bool alwaysBounce;
	public float speed;
	public float amplitud;

	float totalTime;

	bool bouncing;
	Text text;
	float elapsedTime;
	Vector3 originalScale;

	void Start()
	{
		this.text = this.GetComponent<Text> ();
		this.elapsedTime = 0;
		if (alwaysBounce) {
			this.totalTime = -1;
			this.bouncing = true;
		} else {
			this.bouncing = false;
		}
	}

	// Use this for initialization
	public void Bounce (float time) {
		if (bouncing) {
			return;
		}

		this.totalTime = time;
		this.elapsedTime = 0;
		this.bouncing = true;
		this.originalScale = this.transform.localScale;
	}

	// Update is called once per frame
	void Update () {
		if (bouncing) {
			elapsedTime += Time.deltaTime;

			float sin = Mathf.Sin (elapsedTime * speed);
			float result = amplitud * sin;
			text.transform.localScale += Vector3.one * result;

			if (elapsedTime > totalTime && totalTime != -1) {
				bouncing = false;
				this.transform.localScale = originalScale;
			}
		}
	}
}
