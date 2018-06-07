using UnityEngine;
using System.Collections;

public class AI{

	private UnitBase m_Unit;

	public AI()
	{
		m_Unit = null;	
	}

	public IEnumerator StartAI()
	{
		if (m_Unit.GetAct () != E_ACT.DIE) {
			yield return new WaitForSeconds (0.5f);
			MoveUnit ();
			yield return new WaitForSeconds (0.5f);
			if(m_Unit.GetAct()==E_ACT.ATK)
			{
				yield return new WaitForSeconds (0.8f);
				if (m_Unit.GetAct () == E_ACT.COUNTERHIT) {
					m_Unit.GetAnim ().speed=0.0f;
					yield return new WaitForSeconds (3.0f);
				}
				m_Unit.GetAnim ().speed = 1.0f;
				GameObject obj=ObjectPool.GetInst ().GetObject (m_Unit.GetAttackBase ().objType);
				obj.SetActive (false);
				yield return new WaitForSeconds (0.2f);
				if(m_Unit.gameObject.activeSelf)
				{
					obj.GetComponent<AtkElement> ().SetUnit (m_Unit);
					obj.GetComponent<AtkElement> ().SetValue (m_Unit.GetAttackBase ().nDmg);
					obj.SetActive (true);
					m_Unit.GetAnim ().SetInteger ("CurAnim",(int)E_ACT.IDLE);
				}
				yield return new WaitForSeconds (2.0f);
			}
			m_Unit.SetAct (E_ACT.IDLE);
			UnitMgr.GetInst ().TurnOver ();
			yield return null;
		} 
		yield return null;
	}

	private void MoveUnit()
	{
		Panel panel = MapMgr.GetInst ().GetMapPanel (m_Unit.GetCurPanel ().GetPoint ().nX, UnitMgr.GetInst ().GetPlayer ().GetCurPanel ().GetPoint ().nZ);
		UnitMgr.GetInst ().MoveUnit (m_Unit, m_Unit.GetCurPanel (), panel);

		if (m_Unit.GetCurPanel ().GetPoint ().nZ != UnitMgr.GetInst ().GetPlayer ().GetCurPanel ().GetPoint ().nZ) {
			m_Unit.SetAct (E_ACT.IDLE);
			return;
		} 
			

		m_Unit.SetAct (E_ACT.ATK);
		AttackUnit ();

	}

	private void AttackUnit()
	{
		if(m_Unit.GetAct()!=E_ACT.ATK)
		{
			m_Unit.SetAct (E_ACT.IDLE);
			return;
		}
		if(m_Unit.gameObject.activeSelf==false)
		{
			return;
		}
		m_Unit.GetAnim ().SetInteger ("CurAnim", (int)m_Unit.GetAct ());

	}

	public void SetUnit(UnitBase unit)
	{
		m_Unit = unit;
	}
		
}
