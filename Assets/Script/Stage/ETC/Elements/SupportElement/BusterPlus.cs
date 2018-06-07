using UnityEngine;
using System.Collections;

public class BusterPlus : SupportElement {

	void Awake()
	{
		
	}

	void OnEnable()
	{
		StartCoroutine (Work ());
	}
	void OnDisable()
	{

	}

	protected override IEnumerator Work ()
	{
		yield return new WaitUntil (()=>m_Unit!=null);

		UnitAtk atk = m_Unit.GetAttackBase ();

		atk.nDmg += 1;
		atk.fSpeed += 1.0f;

		m_Unit.SetAttackBaseData (atk);

		m_Unit.SetMaxChargeTime (m_Unit.GetMaxChargeTime()-0.3f);

		yield return new WaitForSeconds (1.5f);

		ObjectPool.GetInst ().PooledObject (this.transform.gameObject);

		yield return null;
	}
}
