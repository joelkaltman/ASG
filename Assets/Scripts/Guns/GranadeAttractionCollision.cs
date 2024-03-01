using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GranadeAttractionCollision : PlayerOwned {

	public int duration;
	public GameObject attractionObject;

	GameObject attractionInstance;
	
	void OnTriggerEnter(Collider col)
	{
		if (col.gameObject.GetComponent<PlayerMovement>() != null)
			return;
		
		this.GetComponent<AudioSource>().Play ();

		attractionInstance = Instantiate (attractionObject, new Vector3(this.transform.position.x, 6, this.transform.position.z), Quaternion.identity);
		attractionInstance.GetComponent<AttractEnemies>().player = player;
		
		attractionInstance.GetComponent<NetworkObject>()?.Spawn(true);

		this.GetComponent<Collider> ().enabled = false;
		this.GetComponent<Renderer> ().enabled = false;
		
		StartCoroutine(Destroy());
	}

	IEnumerator Destroy()
	{
		yield return new WaitForSeconds(duration);
		attractionInstance.GetComponent<NetworkObject>()?.Despawn();
		gameObject.GetComponent<NetworkObject>()?.Despawn();
	}
}
