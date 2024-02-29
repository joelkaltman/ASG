using System.Collections;
using UnityEngine;

public class DestroyTimer : ServerOnlyMonobehavior {

	public int destroySeconds;

	void Start () {
		StartCoroutine(DestroyObject());
	}

	IEnumerator DestroyObject()
	{
		yield return new WaitForSeconds(destroySeconds);
		Destroy (this.gameObject);
	}
}
