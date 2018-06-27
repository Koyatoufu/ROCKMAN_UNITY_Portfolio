using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitMgr
{

	protected static UnitMgr m_Inst=null;
    public static UnitMgr Inst { get { return m_Inst; } }

	private GameObject m_goPlayer = null;
	private GameObject m_goEnemy = null;

	protected UnitBase m_player = null;
    public UnitBase Player
    {
        get { return m_player; }
        set
        {
            if (value == null)
            {
                m_player = null;
                return;
            }

            if (m_player != null)
                OtherPlayer = value;

            m_player = value;
        }
    }

    protected UnitBase m_otherPlayer = null;
    public UnitBase OtherPlayer
    {
        get { return m_otherPlayer; }
        set
        {
            if(value==null)
            {
                m_otherPlayer = null;
                return;
            }

            if (m_otherPlayer != null)
                return;

            m_otherPlayer = value;
        }
    }

	private List<UnitBase> m_EnemyList = null;

	private int m_nCurTurn = 0;

	public static void InitInst()
	{
		if(m_Inst==null)
		{
			m_Inst=new UnitMgr();
		}
	}

	public virtual void Initialize()
	{
        m_goPlayer = (GameObject)Resources.Load("Prefebs/Char/Player/RockManExe");
        ObjectPool.GetInst().SetPrefabs(m_goPlayer, 1);

        if (MultyManager.Inst != null)
            return;

        m_goEnemy = (GameObject)Resources.Load("Prefebs/Char/Enemy/Mettol");
        ObjectPool.GetInst().SetPrefabs(m_goEnemy, 3);
    }

	public void MoveUnit(UnitBase unit,Panel pStart,Panel pDest)
    {
		if (!pDest.Passable)
			return;

        if (unit.IsRed != pDest.IsRed)
        {
            return;
        }   
			
		if(unit.photonView!=null&&unit.photonView.isMine)
        {
            unit.photonView.RPC("MovePanel",PhotonTargets.AllBufferedViaServer, pDest.GetPoint().nX , pDest.GetPoint().nZ);
            return;
        }

        unit.SetCurPanel(pDest);
        unit.transform.position = pDest.transform.position;
	}

	public void MoveUnitPath(UnitBase unit,Panel pStart,Panel pDest)
	{
		if(MapMgr.Inst.IsReachable(pStart,pDest)==false){
			Debug.Log("Isn't Reachable");
			return;
		}
		
		if(pDest.Passable){
			if(unit.IsRed!=pDest.IsRed)
				return;
			unit.SetMovePanels(MapMgr.Inst.GetPathPanel(pStart,pDest));
			unit.SetAct(E_ACT.MOVE);
		}
	}

	public List<UnitBase> GetEnemyList()
	{
		return m_EnemyList;
	}

	public IEnumerator GenUnit()
	{
		GameObject Players = new GameObject ("Players");

		m_player = ObjectPool.GetInst().GetObject(m_goPlayer).GetComponent<PlayerUnit> ();
		m_player.SetCurPanel (MapMgr.Inst.GetMapPanel (-2,0));
		m_player.transform.position = m_player.GetCurPanel().transform.position;
		m_player.transform.parent = Players.transform;
		m_player.transform.Rotate(0.0f,90.0f,0.0f);
		m_player.gameObject.SetActive (true);

		m_EnemyList = new List<UnitBase> ();
		m_EnemyList.Capacity = 3;

		yield return null;
		GameObject Enemys = new GameObject ("Enemys");
		

		for(int i=0;i<3;i++)
        {
            Enemy enemyUnit = ObjectPool.GetInst().GetObject(m_goEnemy).GetComponent<Enemy>();
            enemyUnit.SetCurPanel(MapMgr.Inst.GetMapPanel(i, Random.Range(-1, 2)));
            enemyUnit.transform.position = enemyUnit.GetCurPanel().transform.position;
            enemyUnit.transform.parent = Enemys.transform;
            enemyUnit.transform.Rotate(0.0f, -90.0f, 0.0f);
            enemyUnit.gameObject.SetActive(true);
            m_EnemyList.Insert(0, enemyUnit);
            yield return new WaitForSeconds(0.5f);
        }

        StageMgr.Inst.StageStart();

		yield return null;
	}
	public void ReleaseUnit()
	{
        if(m_EnemyList!=null)
		    m_EnemyList.Clear ();
	}

	public IEnumerator EnemyPlay()
	{
		while(m_EnemyList.Count!=0)
		{
			if (m_nCurTurn >= m_EnemyList.Count)
				m_nCurTurn = 0;
			yield return m_EnemyList [m_nCurTurn].GetAI ().StartAI ();

			if(m_player.GetAct()==E_ACT.DIE)
			{
                StageMgr.Inst.GameOver();
				yield break;
			}
			yield return null;
		}

        StageMgr.Inst.ClearStage();

		yield return null;
	}

	public void TurnOver()
	{
		m_nCurTurn++;
	}

    public void RpcInit()
    {
        if (MultyManager.Inst == null)
            return;

        if (m_player == null)
        {
            Debug.Log("Player is Null");
            return;
        }
        
        if (m_otherPlayer == null)
        {
            Debug.Log("OtherPlayer is Null");
            return;
        }

        if (m_player.photonView != null)
        {
            m_player.IsRed = m_player.photonView.owner!=PhotonNetwork.masterClient;
            m_player.photonView.RPC("NetInit", PhotonTargets.AllBufferedViaServer);
        }
        if (m_otherPlayer.photonView != null)
        {
            m_otherPlayer.IsRed = m_otherPlayer.photonView.owner != PhotonNetwork.masterClient;
            m_otherPlayer.photonView.RPC("NetInit", PhotonTargets.AllBufferedViaServer);
        }   
    }

    public void CheckNetUnitDie(PlayerUnit unit)
    {
        if (!PhotonNetwork.connected || PhotonNetwork.room == null || PhotonNetwork.room.PlayerCount < PhotonNetwork.room.MaxPlayers)
            return;

        if ( unit==null || unit.GetAct() != E_ACT.DIE || unit.photonView == null)
            return;

        if (StageMgr.Inst.photonView == null)
            return;

        if (unit.photonView.owner == PhotonNetwork.player)
        {
            Debug.Log("Player Death");
            MultyManager.Inst.ShowMessage("You Lose");
            StageMgr.Inst.GameOver();
        }
        else
        {
            Debug.Log("Ohter Death");
            MultyManager.Inst.ShowMessage("You Win");
            StageMgr.Inst.ClearStage();
        }
    }
}


