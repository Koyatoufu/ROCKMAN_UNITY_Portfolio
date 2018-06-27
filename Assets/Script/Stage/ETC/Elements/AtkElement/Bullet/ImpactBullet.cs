using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpactBullet:AtkElement
{

	private GameObject m_goExplosion = null;

	void Awake()
	{
		m_nValue = 0;
		m_fSpeed = 30.0f;
	}
	void OnEnable()
	{
		m_backParent = transform.parent;
		m_bAllive = true;
		StartCoroutine (ExecuteCoroutine ());
	}
	void OnDisable()
	{
		m_goExplosion = null;
		m_bAllive = false;
		m_Unit = null;
	}
	void OnTriggerEnter(Collider collider)
	{
		UnitBase pBase=collider.GetComponent<UnitBase> ();

		if(pBase==null)
			return;

		if(pBase==m_Unit)
			return;

		m_goExplosion=ObjectPool.GetInst ().GetObject (EffectMgr.GetInst ().GetEffect ((int)E_EFFECTID.IMPACTEXPLOSION));
		m_goExplosion.transform.position = transform.position;

        DamageFunc(pBase);

		m_bAllive = false;
		StopCoroutine (ExecuteCoroutine ());
		PooledThis ();
	}



	protected override IEnumerator ExecuteCoroutine()
	{
		yield return new WaitUntil (() => m_Unit != null);
		transform.parent = null;
		transform.forward = m_Unit.transform.forward;
		transform.position = m_Unit.transform.position + transform.forward+new Vector3(0.0f,1.0f);
		if(m_Unit.GetTransformAtk()!=null)
		{
			transform.position = m_Unit.GetTransformAtk ().position;
		}
		while(m_bAllive)
		{
			transform.position += transform.forward*Time.deltaTime*m_fSpeed;
			if(Vector3.Distance(transform.position,m_Unit.transform.position)>=11.0f)
			{
				PooledThis();
				m_Unit = null;
				yield break;
			}
			yield return null;
		}
		yield return null;
	}

	public override void PooledThis ()
	{
		if(m_goExplosion!=null)
		{
			StageMgr.Inst.StartCoroutine (PooledExplosion());
		}
		if(m_backParent!=null)
		{
			transform.parent = m_backParent;
			if(m_backParent.parent!=null)
			{
				ObjectPool.GetInst ().PooledObject (m_backParent.parent.gameObject);
				return;	
			}
		}
		base.PooledThis ();
	}

	private IEnumerator PooledExplosion()
	{
		
		yield return new WaitForSeconds (2.0f);
		if(m_goExplosion!=null)
		{
			ObjectPool.GetInst ().PooledObject (m_goExplosion);
		}
		yield return null;
	}
}


