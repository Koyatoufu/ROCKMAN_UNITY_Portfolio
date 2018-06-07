using System;
using System.Collections;
using UnityEngine;

public class VulcanBullet:AtkElement
{
	protected VulcanBullet ()
	{
		
	}

	private Transform m_Hit;

	void Awake()
	{
		m_fSpeed = 25.0f;
		m_Hit = this.transform.GetChild(0);
		m_backParent = transform.parent;
	}

	// Use this for initialization
	void Start () {

	}

	void OnEnable()
	{
		m_Hit.gameObject.SetActive (false);
		m_bAllive = true;
		StartCoroutine (Work ());
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
		if(pBase.GetType()==m_Unit.GetType())
			return;
		pBase.GetDamage (m_nValue);
		m_Hit.gameObject.SetActive (true);
		StopCoroutine (Work ());
		Invoke ("PooledThis",0.1f);
	}

	protected override IEnumerator Work ()
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

	protected override void PooledThis ()
	{
		transform.parent = m_backParent;
		transform.gameObject.SetActive(false);
	}
}

