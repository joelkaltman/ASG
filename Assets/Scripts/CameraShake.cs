using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour {

	public float duration;
	public float amount;

	bool shaking;
	float elapsedTime;

	Quaternion originalRotXY;

	public void Shake(float duration, float amount)
	{
		this.originalRotXY = this.transform.rotation;
		this.duration = duration;
		this.amount = amount;
		this.shaking = true;
		this.elapsedTime = 0;
	}

	void Update () {
		elapsedTime += Time.deltaTime;

		if (elapsedTime < duration && shaking) {
			Vector3 rotXY = Random.insideUnitCircle * amount;
			this.transform.Rotate (rotXY);
		} else if (shaking) {
			this.transform.rotation = originalRotXY;
			shaking = false;
		}
	}
}