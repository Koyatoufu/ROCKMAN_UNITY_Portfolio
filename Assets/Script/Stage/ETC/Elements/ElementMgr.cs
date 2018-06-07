using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementMgr
{

	private static ElementMgr m_Inst = null;

	private List<GameObject> m_arGoElement;

	private ElementMgr ()
	{
		m_arGoElement = new List<GameObject> ();
	}

	public static void InitInst()
	{
		if(m_Inst==null)
		{
			m_Inst = new ElementMgr ();
		}
	}

	public static ElementMgr GetInst()
	{
		return m_Inst;
	}

	public void Initialize()
	{
		m_arGoElement.Capacity = (int)E_ATKELEMENT.MAX;

		m_arGoElement.Insert ((int)E_ATKELEMENT.NONE, (GameObject)Resources.Load ("Prefebs/Effect/Bullet"));
		m_arGoElement.Insert ((int)E_ATKELEMENT.BULLET, (GameObject)Resources.Load ("Prefebs/Effect/Bullet"));
		m_arGoElement.Insert ((int)E_ATKELEMENT.CHARGEBULLET, (GameObject)Resources.Load ("Prefebs/Effect/ChargeBullet"));
		m_arGoElement.Insert ((int)E_ATKELEMENT.WAVE, (GameObject)Resources.Load("Prefebs/Effect/Wave"));

		for(int i=0;i<m_arGoElement.Count;i++)
		{
			ObjectPool.GetInst ().SetPrefabs (m_arGoElement[i],5);
		}
	}

	public GameObject GetElement(int nIndex)
	{
		return m_arGoElement [nIndex];
	}

	public void ReleaseElement()
	{
		m_arGoElement.Clear ();
	}
}


