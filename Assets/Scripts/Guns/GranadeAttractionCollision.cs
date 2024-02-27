using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GranadeAttractionCollision : NetworkBehaviour {

	public int duration;
	public GameObject attractionObject;

	GameObject attractionObjectInstance;

	void Start()
	{
		if (!IsHost)
		{
			enabled = false;
			return;
		}
	}
	
	void OnTriggerEnter(Collider col)
	{
		if (col.gameObject.GetComponent<PlayerMovement>() != null)
			return;
		
		this.GetComponent<AudioSource>().Play ();

		attractionObjectInstance = Instantiate (attractionObject, new Vector3(this.transform.position.x, 6, this.transform.position.z), Quaternion.identity);
		attractionObjectInstance.GetComponent<NetworkObject>()?.Spawn(true);

		this.GetComponent<Collider> ().enabled = false;
		this.GetComponent<Renderer> ().enabled = false;
		
		StartCoroutine(Destroy());
	}

	IEnumerator Destroy()
	{
		yield return new WaitForSeconds(duration);
		Destroy (attractionObjectInstance);
		Destroy (this.gameObject);
	}
}
