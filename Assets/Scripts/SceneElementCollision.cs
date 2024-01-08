using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneElementCollision : MonoBehaviour {

	public float speed;
	public float initialLimit;
	public float bounce;
	public GameObject particles;
	public Color color;

	private float limit;
	private float rot;
	private float currentDir;
	private float first;
	private bool shaking;
	private Quaternion originalRotXY;

	public void Shake()
	{
		if (shaking) {
			return;
		}

		this.originalRotXY = this.transform.rotation;
		this.rot = 0;
		this.limit = this.initialLimit;
		this.currentDir = 1;
		this.first = 2;

		this.shaking = true;
	}

	void Update () {
		if (shaking) {
			Vector3 aroundPoint = new Vector3 (this.transform.position.x, 5, this.transform.position.z);

			rot += speed;
			if (rot > limit/first) {
				limit -= bounce;
				currentDir = -currentDir;
				rot = 0;
				first = 1;
			}
			this.transform.RotateAround (aroundPoint, Vector3.forward, currentDir * rot);

			if (limit < 0.5f) {
				shaking = false;
				this.transform.rotation = this.originalRotXY;
			}
		}
	}
		
	void OnCollisionEnter(Collision col)
	{
		if (col.gameObject.layer == LayerMask.NameToLayer("Bullet")) {
			Shake ();
			GameObject instancePart = Instantiate (this.particles, col.gameObject.transform.position, Quaternion.identity);

			Vector3 dif = col.gameObject.transform.position - this.transform.position;
			dif.Normalize ();
			dif.y = 2;

			instancePart.transform.position += dif * 0.1f;
			instancePart.transform.rotation = Quaternion.LookRotation (dif);

			ParticleSystem.MainModule particles = instancePart.GetComponent<ParticleSystem> ().main;
			particles.startColor = new Color(this.color.r, this.color.g, this.color.b);
			Destroy (instancePart, 1);
		}
	}
}
