﻿using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class DestroyTimer : MonoBehaviour {

	public int destroySeconds;

	// Use this for initialization
	void Start () {
		StartCoroutine(DestroyObject());
	}

	IEnumerator DestroyObject()
	{
		yield return new WaitForSeconds(destroySeconds);
		Destroy (this.gameObject);
	}
}