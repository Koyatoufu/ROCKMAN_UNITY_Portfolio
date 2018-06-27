using System;
using System.Collections;
using UnityEngine;

public class Shield:Photon.MonoBehaviour
{
	void OnTriggerEnter(Collider collider)
	{
		if (collider.GetComponent<AtkElement> () == null)
			return;
		ObjectPool.GetInst ().PooledObject (collider.gameObject);
	}
}



