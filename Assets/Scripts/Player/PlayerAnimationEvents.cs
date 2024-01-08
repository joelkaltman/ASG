using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour {


	public void PostDead()
	{
		this.GetComponent<Animator>().SetBool("Died", true);
	}


	public void throwGranade()
	{
		this.GetComponent<PlayerStats> ().ThrowGranade ();
	}
}
