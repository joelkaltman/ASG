using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GranadeAttractionCollision : MonoBehaviour {

	public int duration;
	public GameObject attractionObject;

	GameObject attractionObjectInstance;

	void OnCollisionEnter(Collision col)
	{
		this.GetComponent<AudioSource>().Play ();

		attractionObjectInstance = Instantiate (attractionObject, new Vector3(this.transform.position.x, 6, this.transform.position.z), Quaternion.identity);
		Invoke ("Destroy", duration);

		this.GetComponent<Collider> ().enabled = false;
		this.GetComponent<Renderer> ().enabled = false;
	}

	void Destroy()
	{
		Destroy (attractionObjectInstance);
		Destroy (this.gameObject);
	}
}
