using UnityEngine;
using System.Collections;

public class BusterPlus : SupportElement
{
	void OnEnable()
	{
		StartCoroutine (ExecuteCoroutine ());
	}

	protected override IEnumerator ExecuteCoroutine ()
	{
		yield return new WaitUntil (()=>m_Unit!=null);

		UnitAtk atk = m_Unit.GetAttackBase ();

		atk.nDmg += 1;
		atk.fSpeed += 1.0f;

		m_Unit.SetAttackBaseData (atk);

		m_Unit.MaxChargeTime = m_Unit.MaxChargeTime-0.3f;

		yield return new WaitForSeconds (1.5f);

		ObjectPool.GetInst ().PooledObject (this.transform.gameObject);

		yield return null;
	}
}
