using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TransparentOnOverlap : MonoBehaviour {

	public Material Transparent;

	GameObject player;
	Renderer rend;

	Material normalMaterial;

	bool isTransparent;

	// Use this for initialization
	void Start()
	{
		rend = this.GetComponent<Renderer>();
		normalMaterial = rend.material;
		isTransparent = false;
		
		PlayerSpawn.Instance.AddListener(OnPlayerSpawn);
	}
	
	private void OnDestroy()
	{
		PlayerSpawn.Instance.RemoveListener(OnPlayerSpawn);
	}

	private void OnPlayerSpawn(GameObject spawned)
	{
		if (!GameData.Instance.isOnline || spawned.GetComponent<NetworkObject>().IsOwner)
		{
			player = spawned;
		}
	}

	// Update is called once per frame
	void Update () 
	{
		if (!player)
			return;
		
		Vector3 directionRay = player.transform.position - Camera.main.transform.position;

		RaycastHit hit;
		Ray ray = new Ray(Camera.main.transform.position, directionRay);
		if (Physics.Raycast (ray, out hit)) {
			if (GameObject.ReferenceEquals(hit.collider.gameObject, this.gameObject)) {
				if (!isTransparent) {
					rend.material = Transparent;
					isTransparent = true;
				}
			} else {
				rend.material = normalMaterial;
				isTransparent = false;
			}
		} else {
			rend.material = normalMaterial;
			isTransparent = false;
		}
	}
}
