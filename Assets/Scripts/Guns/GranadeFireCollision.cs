using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GranadeFireCollision : MonoBehaviour {

	public int duration;
	public GameObject fire;

	GameObject fireInstance;

	void OnCollisionEnter(Collision col)
	{
		this.GetComponent<AudioSource>().Play ();

		fireInstance = Instantiate (fire, new Vector3(this.transform.position.x, 5, this.transform.position.z), Quaternion.Euler(-90,0,0));
		Invoke ("EndFire", duration);

		this.GetComponent<Collider> ().enabled = false;
		this.GetComponent<Renderer> ().enabled = false;
	}

	void EndFire()
	{
		fireInstance.GetComponent<ParticleSystem> ().Stop();
		Invoke ("DestroyFire", 1);
	}

	void DestroyFire()
	{
		Destroy (fireInstance);
		Destroy (this.gameObject);
	}
}
