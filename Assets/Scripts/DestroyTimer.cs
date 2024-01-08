using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyTimer : MonoBehaviour {

	public int destroySeconds;

	// Use this for initialization
	void Start () {
		Invoke ("DestroyObject", destroySeconds);
	}

	void DestroyObject()
	{
		Destroy (this.gameObject);
	}
}
