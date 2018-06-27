using System;
using System.Collections;
using UnityEngine;
public class Barrier:SupportElement
{
	protected Material m_material = null;

	void Awake()
	{
        m_nValue = 0;
        m_Unit = null;
        m_material = transform.GetComponent<Material> ();
	}

	void OnEnable()
	{
		StartCoroutine (Appear ());
	}
	void OnDisable()
	{
		m_Unit = null;
	}

	void OnTriggerEnter(Collider collider)
	{		
		AtkElement atkTmp=collider.GetComponent<AtkElement>();
		if (atkTmp == null)
			return;
        if (MultyManager.Inst == null)
        {
            if (atkTmp.GetUnit().GetType() == m_Unit.GetType())
                return;
        }
        else
        {
            if (atkTmp.GetUnit() == m_Unit)
                return;
        }

        m_nValue -= atkTmp.GetValue ();
        atkTmp.PooledThis();
		//ObjectPool.GetInst ().PooledObject (collider.transform.gameObject);
		if(m_nValue<0)
		{
            PooledThis();
			//ObjectPool.GetInst ().PooledObject (transform.gameObject);
		}
	}

	protected IEnumerator Appear()
	{
		yield return new WaitUntil(()=>m_material!=null);
        yield return new WaitUntil(()=>m_Unit!=null);

		for(float f=0.0f;f<1.0f;f+=0.025f)
		{
			m_material.color = new Vector4 (1.0f, 1.0f, 1.0f, f);
			yield return null;
		}
		yield return null;
	}
}

