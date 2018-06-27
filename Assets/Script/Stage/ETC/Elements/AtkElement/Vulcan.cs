using UnityEngine;
using System.Collections;

public class Vulcan : AtkElement {

	GameObject [] m_argoBullets = null;

	void Awake()
	{
		if(transform.childCount>0)
		{
			m_argoBullets = new GameObject[transform.childCount];
			for(int i=0;i<transform.childCount;i++)
			{
				m_argoBullets[i] = transform.GetChild (i).gameObject;
			}
		}
	}

	void OnEnable()
	{
		m_backParent = transform.parent;
		m_bAllive = true;
		StartCoroutine (ExecuteCoroutine ());
	}
	void OnDisable()
	{
		m_Unit = null;
		m_bAllive = false;
	}

	protected override IEnumerator ExecuteCoroutine ()
	{
		yield return new WaitUntil (()=>m_Unit!=null);
		transform.parent = null;
		transform.position = m_Unit.transform.position + transform.forward + transform.up;
		if(m_Unit.GetTransformAtk()!=null)
			transform.position = m_Unit.GetTransformAtk ().position;
		for(int i=0;i<m_argoBullets.Length;i++)
		{
			m_argoBullets [i].SetActive (true);
			AtkElement atkTmp = m_argoBullets [i].GetComponent<AtkElement> ();
			atkTmp.SetUnit (m_Unit);
			atkTmp.SetValue (m_nValue);
			yield return new WaitForSeconds (0.1f);
		}
		yield return new WaitForSeconds(0.5f);

		while(m_bAllive)
		{
			if(transform.childCount==m_argoBullets.Length)
			{
				PooledThis ();
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
	}
}
