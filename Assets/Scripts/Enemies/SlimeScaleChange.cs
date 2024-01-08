using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeScaleChange : MonoBehaviour {

	public float speed;
	public float amplitud;

	float elapsedTime;

	void Start(){
		elapsedTime = 0;
	}

	// Update is called once per frame
	void Update () {
		elapsedTime += Time.deltaTime;

		Vector3 scale = this.transform.localScale;
		scale.y = 1.5f + amplitud * Mathf.Sin (speed * elapsedTime);

		this.transform.localScale = scale;
	}
}
