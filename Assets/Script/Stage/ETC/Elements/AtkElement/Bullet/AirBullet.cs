using System;
using System.Collections;
using UnityEngine;

public class AirBullet:AtkElement
{
	protected AirBullet ()
	{
	}

	void Awake()
	{
		m_nValue = 0;
		m_fSpeed = 50.0f;
	}
	// Use this for initialization
	void Start () {

	}

	void OnEnable()
	{
		m_backParent = transform.parent;
		m_bAllive = true;
		StartCoroutine (Work());
	}
	void OnDisable()
	{
		m_bAllive = false;
		m_Unit = null;
	}
	void OnTriggerEnter(Collider collider)
	{
		UnitBase pBase=collider.GetComponent<UnitBase> ();

		if(pBase==null)
			return;

		if(pBase.GetType()==m_Unit.GetType())
			return;

		pBase.GetDamage (m_nValue);
		int nX = 1;
		if(transform.forward.x<0)
		{
			nX = -1;
		}

		Panel pDest = MapMgr.GetInst().GetMapPanel(pBase.GetCurPanel().GetPoint().nX+nX,pBase.GetCurPanel().GetPoint().nZ);
		UnitMgr.GetInst ().MoveUnit (pBase, pBase.GetCurPanel (), pDest);
		m_bAllive = false;
		StopCoroutine (Work ());

		PooledThis ();
	}

	protected override IEnumerator Work ()
	{
		yield return new WaitUntil(()=>m_Unit!=null);
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
				yield break;
			}
			yield return null;
		}

		yield return null;
	}

	protected override void PooledThis ()
	{
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
}


