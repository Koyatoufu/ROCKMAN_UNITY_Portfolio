using System;
using System.Collections;
using UnityEngine;

public class AirBullet:AtkElement
{
	void Awake()
	{
		m_nValue = 0;
		m_fSpeed = 50.0f;
	}

	void OnEnable()
	{
		m_backParent = transform.parent;
		m_bAllive = true;
		StartCoroutine (ExecuteCoroutine());
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

		if(pBase==m_Unit)
			return;

        int nX = 1;
        if (transform.forward.x < 0)
        {
            nX = -1;
        }

        DamageFuncMove(pBase,nX);

		m_bAllive = false;
		StopCoroutine (ExecuteCoroutine ());

		PooledThis ();
	}

    protected override IEnumerator ExecuteCoroutine ()
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

	public override void PooledThis ()
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


