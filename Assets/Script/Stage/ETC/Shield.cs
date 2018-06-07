using System;
using System.Collections;
using UnityEngine;

public class Shield:MonoBehaviour
{
	private Shield ()
	{
		
	}

	void onEneble()
	{
		
	}

	void OnTriggerEnter(Collider collider)
	{
		if (collider.GetComponent<AtkElement> () == null)
			return;
		ObjectPool.GetInst ().PooledObject (collider.gameObject);
	}
}



