using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class GranadeFireCollision : PlayerOwned {

	public int duration;
	public GameObject fire;

	GameObject fireInstance;
	
	void OnTriggerEnter(Collider col)
	{
		if (col.gameObject.GetComponent<PlayerMovement>() != null)
			return;
		
		this.GetComponent<AudioSource>().Play ();

		fireInstance = Instantiate (fire, new Vector3(this.transform.position.x, 5, this.transform.position.z), Quaternion.Euler(-90,0,0));
		fireInstance.GetComponent<FireCollision>().player = player;
		
		fireInstance.GetComponent<NetworkObject>()?.Spawn(true);

		this.GetComponent<Collider> ().enabled = false;
		this.GetComponent<Renderer> ().enabled = false;
		
		StartCoroutine(EndFire());
	}

	IEnumerator EndFire()
	{
		yield return new WaitForSeconds(duration);
		fireInstance.GetComponent<ParticleSystem> ().Stop();
		StartCoroutine(DestroyFire());
	}

	IEnumerator DestroyFire()
	{
		yield return new WaitForSeconds(1);
		fireInstance.GetComponent<NetworkObject>()?.Despawn(true);
		gameObject.GetComponent<NetworkObject>()?.Despawn(true);
	}
}
