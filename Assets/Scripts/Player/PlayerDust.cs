using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDust : MonoBehaviour
{
	public GameObject dust;
	
	private PlayerStats playerStats;
	private PlayerMovement playerMovement;
	
    void Start()
    {
	    playerStats = GetComponent<PlayerStats>();
	    playerMovement = GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
		if (playerStats.Speed.Value > playerStats.initialSpeed && playerMovement.IsMoving && dust) {
			var dustInstance = Instantiate (dust, transform.position, Quaternion.identity);
			Destroy (dustInstance, 3);
		}
    }
}
