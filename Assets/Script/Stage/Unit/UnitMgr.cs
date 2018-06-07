using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitMgr{
	private static UnitMgr m_Inst=null;

	private GameObject m_goPlayer;
	private GameObject m_goEnemy0;

	private UnitBase m_Player;
	private List<UnitBase> m_EnemyList;

	private int m_nCurTurn;

	private UnitMgr()
	{
		m_goPlayer = null;
		m_goEnemy0 = null;
		m_EnemyList = null;
		m_nCurTurn = 0;
	}

	public static void InitInst()
	{
		if(m_Inst==null)
		{
			m_Inst=new UnitMgr();
		}
	}
	public static UnitMgr GetInst()
	{
		return m_Inst;

	}

	public void Initialize()
	{
		m_goEnemy0 = (GameObject)Resources.Load ("Prefebs/Char/Enemy/Mettol");
		m_goPlayer = (GameObject)Resources.Load ("Prefebs/Char/Player/RockManExe");

		ObjectPool.GetInst ().SetPrefabs (m_goEnemy0,3);
		ObjectPool.GetInst ().SetPrefabs (m_goPlayer,1);
	}
	public void MoveUnit(UnitBase unit,Panel pStart,Panel pDest){
		if (!pDest.GetPassable ())
			return;

		if (unit is PlayerUnit && pDest.GetTagName() == "EnemyArea")
			return;
		else if (unit is Enemy && pDest.GetTagName() == "PlayerArea")
			return;
			
		unit.SetCurPanel (pDest);
		unit.transform.position = pDest.transform.position;
	}

	public void MoveUnitPath(UnitBase unit,Panel pStart,Panel pDest)
	{
		if(MapMgr.GetInst().IsReachable(pStart,pDest)==false){
			Debug.Log("Isn't Reachable");
			return;
		}
		//int dis = MapMgr.GetInst().GetDistance (pStart.GetPoint(), pDest.GetPoint());
		if(pDest.GetPassable()==true){
			if(unit.GetOppoPanel().CompareTo(pDest.GetTagName())==0)
				return;
			unit.SetMovePanels(MapMgr.GetInst().GetPathPanel(pStart,pDest));
			unit.SetAct(E_ACT.MOVE);
		}
	}

	public UnitBase GetPlayer()
	{
		return m_Player;
	}
	public List<UnitBase> GetEnemyList()
	{
		return m_EnemyList;
	}

	public IEnumerator GenUnit()
	{
		GameObject Players = new GameObject ("Players");

		m_Player = ObjectPool.GetInst().GetObject(m_goPlayer).GetComponent<PlayerUnit> ();
		m_Player.SetCurPanel (MapMgr.GetInst ().GetMapPanel (-2,0));
		m_Player.transform.position = m_Player.GetCurPanel().transform.position;
		m_Player.transform.parent = Players.transform;
		m_Player.transform.Rotate(0.0f,90.0f,0.0f);
		m_Player.gameObject.SetActive (true);

		m_EnemyList = new List<UnitBase> ();
		m_EnemyList.Capacity = 3;

		yield return null;
		GameObject Enemys = new GameObject ("Enemys");
		Enemy enemyUnit = ObjectPool.GetInst().GetObject(m_goEnemy0).GetComponent<Enemy> ();
		enemyUnit.SetCurPanel (MapMgr.GetInst ().GetMapPanel (1,-1));
		enemyUnit.transform.position = enemyUnit.GetCurPanel().transform.position;
		enemyUnit.transform.parent = Enemys.transform;
		enemyUnit.transform.Rotate(0.0f,-90.0f,0.0f);
		enemyUnit.gameObject.SetActive (true);
		m_EnemyList.Insert (0,enemyUnit);
		yield return new WaitForSeconds (0.5f);

		enemyUnit = ObjectPool.GetInst().GetObject(m_goEnemy0).GetComponent<Enemy> ();
		enemyUnit.SetCurPanel (MapMgr.GetInst ().GetMapPanel (1,1));
		enemyUnit.transform.position = enemyUnit.GetCurPanel().transform.position;
		enemyUnit.transform.parent = Enemys.transform;
		enemyUnit.transform.Rotate(0.0f,-90.0f,0.0f);
		enemyUnit.gameObject.SetActive (true);
		m_EnemyList.Insert (1,enemyUnit);
		yield return new WaitForSeconds (0.5f);

		enemyUnit = ObjectPool.GetInst().GetObject(m_goEnemy0).GetComponent<Enemy> ();
		enemyUnit.SetCurPanel (MapMgr.GetInst ().GetMapPanel (2,0));
		enemyUnit.transform.position = enemyUnit.GetCurPanel().transform.position;
		enemyUnit.transform.parent = Enemys.transform;
		enemyUnit.transform.Rotate(0.0f,-90.0f,0.0f);
		enemyUnit.gameObject.SetActive (true);
		m_EnemyList.Insert (2,enemyUnit);
		yield return new WaitForSeconds (0.5f);

		StageMgr.GetInst ().StartCoroutine (StageMgr.GetInst().SetStart());

		yield return null;
	}
	public void ReleaseUnit()
	{
		m_EnemyList.Clear ();
	}

	public IEnumerator EnemyPlay()
	{
		while(m_EnemyList.Count!=0)
		{
			if (m_nCurTurn >= m_EnemyList.Count)
				m_nCurTurn = 0;
			yield return m_EnemyList [m_nCurTurn].GetAI ().StartAI ();
			if(m_Player.GetAct()==E_ACT.DIE)
			{
				StageMgr.GetInst ().StartCoroutine (StageMgr.GetInst().GameOver());
				yield break;
			}
			yield return null;
		}
		StageMgr.GetInst ().StartCoroutine (StageMgr.GetInst().ClearStage());
		yield return null;
	}

	public void TurnOver()
	{
		m_nCurTurn++;
	}
}


