using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EffectMgr {

	private static EffectMgr m_Inst=null;

	private List<GameObject> m_Effects;

	public static void InitInst()
	{
		if(m_Inst==null)
		{
			m_Inst = new EffectMgr ();			
		}
	}

	public static EffectMgr GetInst()
	{
		return m_Inst;
	}

	private EffectMgr()
	{
		m_Effects = new List<GameObject>();
	}

	public void Initailzie()
	{
		m_Effects.Capacity = (int)E_EFFECTID.MAX;
		m_Effects.Insert ((int)E_EFFECTID.SHIELD, (GameObject)Resources.Load ("Prefebs/Effect/Shield"));
		m_Effects.Insert ((int)E_EFFECTID.LOCKON, (GameObject)Resources.Load ("Prefebs/Effect/Lockon"));
		m_Effects.Insert ((int)E_EFFECTID.EXPLOSION, (GameObject)Resources.Load ("Prefebs/Effect/Explosion"));
		m_Effects.Insert ((int)E_EFFECTID.IMPACTEXPLOSION, Resources.Load<GameObject> ("Prefebs/Effect/ImpactExplosion"));
		m_Effects.Insert ((int)E_EFFECTID.POOF, Resources.Load<GameObject> ("Prefebs/Effect/Poof"));
		m_Effects.Insert ((int)E_EFFECTID.DEATH, (GameObject)Resources.Load ("Prefebs/Effect/Death"));
	}

	public void AddEffect(GameObject obj)
	{
		for(int i=0;i<m_Effects.Count;i++)
		{
			if(m_Effects[i].name.CompareTo(obj.name)==0)
			{
				return;
			}
		}

		m_Effects.Capacity = m_Effects.Capacity + 1;

		m_Effects.Insert (m_Effects.Count,obj);
	}

	public void SetEffectPooled()
	{
		for(int i=0;i<m_Effects.Count;i++)
		{
			ObjectPool.GetInst ().SetPrefabs (m_Effects[i],5);
		}
			
	}

	public GameObject GetEffect(int nIndex)
	{
		return m_Effects [nIndex];
	}

	public void ReleaseEffect()
	{
		m_Effects.Clear ();
	}
}
