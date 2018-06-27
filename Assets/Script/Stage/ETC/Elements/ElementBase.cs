using UnityEngine;
using System.Collections;

public class ElementBase : Photon.MonoBehaviour {

	protected Rigidbody m_rigidBody;

	protected UnitBase m_Unit;

	protected int m_nValue;
	protected bool m_bAllive;

	protected Transform m_backParent;

	protected ElementBase()
	{
		m_rigidBody = null;
		m_Unit = null;

		m_nValue = 0;
		m_bAllive = false;

		m_backParent = null;
	}

	protected virtual IEnumerator ExecuteCoroutine()
	{
		yield return null;
	}

	public virtual void PooledThis()
	{
		ObjectPool.GetInst ().PooledObject (this.gameObject);
	}

	public void SetUnit(UnitBase unit){
		m_Unit = unit;
	}

	public void SetValue(int nValue)
	{
		m_nValue = nValue;
	}

	public UnitBase GetUnit()
	{
		return m_Unit;
	}

	public int GetValue()
	{
		return m_nValue;
	}


}
