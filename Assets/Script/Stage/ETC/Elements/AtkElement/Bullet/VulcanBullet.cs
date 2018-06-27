using System;
using System.Collections;
using UnityEngine;

public class VulcanBullet:AtkElement
{
	private Transform m_Hit = null;

	void Awake()
	{
		m_fSpeed = 25.0f;
		m_Hit = this.transform.GetChild(0);
		m_backParent = transform.parent;
	}

	void OnEnable()
	{
		m_Hit.gameObject.SetActive (false);
		m_bAllive = true;
		StartCoroutine (ExecuteCoroutine ());
	}
	void OnDisable()
	{
		m_bAllive = false;
	}

	void OnTriggerEnter(Collider collider)
	{
		if (m_Unit == null)
			return;
		UnitBase pBase=collider.GetComponent<UnitBase> ();
		if (pBase == null)
			return;
		if(pBase==m_Unit)
			return;
        DamageFunc(pBase);
		m_Hit.gameObject.SetActive (true);
		StopCoroutine (ExecuteCoroutine ());
		Invoke ("PooledThis",0.1f);
	}

	protected override IEnumerator ExecuteCoroutine ()
	{
		yield return new WaitUntil (()=>m_Unit!=null);
		transform.parent = null;
		//transform.rotation = m_Unit.transform.rotation;
		transform.forward = m_Unit.transform.forward;
		//transform.position = m_Unit.transform.position + transform.forward+new Vector3(0.0f,1.0f);
		yield return null;
		while(m_bAllive)
		{
			transform.position += transform.forward*Time.deltaTime*m_fSpeed;
			if(Vector3.Distance(transform.position,m_Unit.transform.position)>=15.0f)
			{
				PooledThis ();
				yield break;
			}
			yield return null;
		}
		yield return null;
	}

	public override void PooledThis ()
	{
		transform.parent = m_backParent;
		transform.gameObject.SetActive(false);
	}
}

