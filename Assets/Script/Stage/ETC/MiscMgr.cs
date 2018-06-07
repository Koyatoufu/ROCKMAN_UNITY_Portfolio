using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class MiscMgr
{
	private static MiscMgr m_Inst=null;

	private GameObject[] m_arMiscObj;

	private MiscMgr ()
	{
		m_arMiscObj = null;
	}

	public static void InitInst()
	{
		if(m_Inst==null)
		{
			m_Inst=new MiscMgr();
		}
	}
	public static MiscMgr GetInst()
	{
		return m_Inst;
	}

	public void Initialize()
	{
		m_arMiscObj=new GameObject[2];
	}

	public GameObject GetMiscObject(int nId)
	{
		if(m_arMiscObj.Length<nId)
		{
			return null;
		}
		return m_arMiscObj [nId];
	}
}


