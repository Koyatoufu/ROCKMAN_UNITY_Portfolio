using UnityEngine;
using System.Collections;

public class AreaSteal : AreaElement {

	protected GameObject[] m_arGoChild;
	protected AreaElement[] m_arElement;

	void Awake()
	{
		m_nChildCount = transform.childCount;

		m_arGoChild = new GameObject[m_nChildCount];
		m_arElement = new AreaElement[m_nChildCount];

		for(int i=0;i<m_nChildCount;i++)
		{
			m_arGoChild [i] = transform.GetChild (i).gameObject;
			m_arElement [i] = m_arGoChild [i].GetComponent<AreaElement> ();
			m_arGoChild [i].SetActive (false);
		}
	}

	void OnEnable()
	{
		m_bAllive = true;
		StartCoroutine (Work ());
	}
	void OnDisable()
	{
		m_Unit = null;
		m_bAllive = false;
	}

	protected override IEnumerator Work()
	{
		yield return new WaitUntil (()=>m_Unit!=null);

		m_panel = m_Unit.GetCurPanel ();

		int nIndexX = m_panel.GetPoint ().nX;
		int nMaxSizeX = MapMgr.GetInst ().GetSizeX ();

		while(nIndexX<nMaxSizeX)
		{
			nIndexX++;


			Panel pTmp = MapMgr.GetInst ().GetMapPanel (nIndexX, 0);

			if(pTmp.GetTagName().CompareTo(m_panel.GetTagName())!=0)
			{
				transform.position = MapMgr.GetInst ().GetMapPanel (nIndexX, 0 ).transform.position+new Vector3(0.0f,3.0f,0.0f);
				break;
			}
		}
		yield return null;

		for(int i=0;i<m_nChildCount;i++)
		{
			m_arGoChild [i].SetActive(true);
			m_arElement [i].SetUnit (m_Unit);
		}
		yield return null;

		yield return new WaitForSeconds(0.5f);

		while(m_bAllive)
		{
			if(transform.childCount==m_nChildCount)
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
		base.PooledThis ();
	}
}
