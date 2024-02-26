using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtPlayer : MonoBehaviour 
{
	void Update ()
	{
		GameObject player = MultiplayerManager.Instance.GetLocalPlayer();
		if(!player)
			return;
		
		Vector3 dir = player.transform.position - this.transform.position;
		this.transform.rotation = Quaternion.LookRotation (dir);
	}
}
