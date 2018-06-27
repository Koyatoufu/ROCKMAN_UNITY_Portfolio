using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MultyManager : Photon.PunBehaviour
{
    private static MultyManager m_inst = null;
    public static MultyManager Inst { get { return m_inst; } }

    [SerializeField]
    private Text m_messageText = null;
    [SerializeField]
    private GameObject m_messageGo = null;

    void Awake()
    {
        if (m_inst != null)
        {
            Debug.Log("MultyManager Destroy");
            Destroy(gameObject);
            return;
        }
        
        m_inst = this;
    }

    void Start()
    {
        StartCoroutine(MultyStartCoroutine());
    }

    IEnumerator MultyStartCoroutine()
    {
        Room room = PhotonNetwork.room;

        float fWaitT = 0f;
        
        while (room == null)
        {
            yield return null;
            room = PhotonNetwork.room;
            fWaitT += Time.deltaTime;

            if(fWaitT>=5f)
            {
                ReturnLobby();
                yield break;
            }
        }
        
        while (MapMgr.Inst==null || !MapMgr.Inst.IsCreated)
            yield return null;

        PlayerUnit player = null;
        PhotonView view = null;

        player = PlayerCreate(room, out view);

        if (CheckPlayerAndViewExecpt(player, view))
        {
            ReturnLobby();
            yield break;
        }

        m_messageText.text = "Hold On Player";
        while (room.PlayerCount != room.MaxPlayers)
        {
            Debug.Log("Player Wait");
            yield return null;
        }

        while(UnitMgr.Inst.OtherPlayer==null)
        {
            OtherPlayerFind();
            yield return null;
        }
                
        StageMgr.Inst.StageStart();

        m_messageText.text = "Player Filled";

        UnitMgr.Inst.RpcInit();
        StartCoroutine(MultyUpdateCoroutine());
        
        yield return null;
    }

    private PlayerUnit PlayerCreate(Room room,out PhotonView view)
    {
        GameObject playerGo = PhotonNetwork.Instantiate("Prefebs/Char/Player/NetRockman", Vector3.zero, Quaternion.identity, 0);
        view = null;

        if (playerGo == null)
            return null;

        PlayerUnit playerUnit = playerGo.GetComponent<PlayerUnit>();

        if (playerUnit == null)
            return null;

        view = playerUnit.photonView;
        if (view == null)
            return null;

        bool isRed = view.ownerId==1 ? false : true ;
        float fRotY = isRed ? -90f : 90f;
        int nXPenel = isRed ? 1 : -2;

        playerUnit.IsRed = isRed;
        playerUnit.SetCurPanel(MapMgr.Inst.GetMapPanel(nXPenel, 0));
        playerUnit.transform.position = playerUnit.GetCurPanel().transform.position;
        playerUnit.transform.rotation = Quaternion.Euler(0f, fRotY, 0f);

        playerUnit.GetAnim().speed = 0f;

        return playerUnit;
    }

    private bool CheckPlayerAndViewExecpt(PlayerUnit player, PhotonView view)
    {
        if (player == null || view == null)
        {
            ReturnLobby();
            return true;
        }

        if (view.owner==PhotonNetwork.player)
            UnitMgr.Inst.Player = player;

        return false;
    }

    private void ReturnLobby()
    {
        if(PhotonNetwork.inRoom)
            PhotonNetwork.LeaveRoom();
        PhotonNetwork.LoadLevel("Lobby");
    }
    
    private void OtherPlayerFind()
    {
        Debug.Log("Other Player Find");

        if (PhotonNetwork.otherPlayers.Length < 1)
            return;

        PhotonView view = PhotonView.Find(PhotonNetwork.otherPlayers[0].ID*1000+1);
        Debug.Log(PhotonNetwork.otherPlayers[0].ID);

        if(view==null)
        {
            Debug.Log("View is Null");
            return;
        }

        PlayerUnit unit = view.GetComponent < PlayerUnit>();

        if (unit == null)
            return;

        UnitMgr.Inst.OtherPlayer = unit;
    }

    public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        Debug.Log("Player Connected");
        m_messageText.text = "New Player Connected";
    }

    public void ShowMessage(string message,float hideTime = 0f)
    {
        if (string.IsNullOrEmpty(message))
            return;

        m_messageGo.SetActive(true);
        m_messageText.text = message;

        if (hideTime!=0f)
        {
            Invoke("HideMessage", hideTime);
        }
    }

    public void HideMessage()
    {
        m_messageGo.SetActive(false);
    }

    IEnumerator MultyUpdateCoroutine()
    {
        yield return null;

        if(m_messageGo!=null)
            m_messageGo.SetActive(false);

        yield return new WaitUntil(() => StageMgr.Inst.IsPlay);

        while(StageMgr.Inst.IsPlay)
        {
            Debug.Log("MultyPlay");

            Debug.Log(PhotonNetwork.room);

            if(PhotonNetwork.room==null||PhotonNetwork.room.PlayerCount<PhotonNetwork.room.MaxPlayers)
            {
                ShowMessage("Player Out, Return Lobby" , 1f);
                yield return new WaitForSeconds(1f);
                ReturnLobby();
                yield break;
            }

            Debug.Log(PhotonNetwork.room.PlayerCount);

            yield return null;
        }
    }
}
