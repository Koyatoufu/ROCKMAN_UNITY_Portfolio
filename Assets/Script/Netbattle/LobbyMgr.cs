using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyMgr : Photon.PunBehaviour
{
    private static LobbyMgr m_inst = null;
    public static LobbyMgr Inst { get { return m_inst; } }

    [SerializeField]
    private Text m_lobbyMessage = null;

    [SerializeField]
    private string m_version = null;

    [SerializeField]
    private GameObject m_lobbyMenugo = null;

    [SerializeField]
    private GameObject m_createMenugo = null;

    [SerializeField]
    private GameObject m_joinMenugo = null;

    [SerializeField]
    private GameObject m_roomListParent = null;

    [SerializeField]
    private GameObject m_roomItemPrefab = null;

    void Awake()
    {
        if (m_inst != null)
        {
            Destroy(gameObject);
            return;
        }

        m_inst = this;

        PhotonNetwork.automaticallySyncScene = true;
    }

    void Start()
    {
        ConnectPhotonCloud();
    }

    void ConnectPhotonCloud()
    {
        if (!PhotonNetwork.connected)
        {
            if(m_lobbyMessage!=null)
                m_lobbyMessage.text = "Photon Croud Connecting...";

            bool isConnect = PhotonNetwork.ConnectUsingSettings(m_version);

            Debug.Log("[INFO] Photon Network Connection: " + isConnect);
        }
        else
        {
            OnJoinedLobby();
        }
    }

    public override void OnJoinedLobby()
    {
        m_lobbyMessage.text = "Now on Lobby";

        if(m_lobbyMenugo!=null)
        {
            m_lobbyMenugo.SetActive(true);
        }
    }

    public void CreateOrJoinRoom(string roomName)
    {
        bool isSuccess = PhotonNetwork.JoinOrCreateRoom(roomName, new RoomOptions() { MaxPlayers = 2, IsOpen = true, IsVisible = true }, TypedLobby.Default);

        Debug.Log("[INFO] Room Create or Join: " + isSuccess);

        if(isSuccess)
        {
            m_lobbyMessage.text = "RoomConnect Success";
        }
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("[INFO] Room Connection Complete");

        m_lobbyMessage.text = "Move to Room";

        PhotonNetwork.LoadLevel("NetworkStage");
    }

    public void ReturnTitle()
    {
        PhotonNetwork.Disconnect();
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainScene");
    }

    public override void OnPhotonCreateRoomFailed(object[] codeAndMsg)
    {
        // 0: 오류 코드, 1: 오류 내용
        Debug.Log("[ERROR] Room Create Error:" + codeAndMsg[1].ToString());
        StartCoroutine(LoadMessageCoroutine(codeAndMsg[1].ToString(), 2f));
    }

    public override void OnPhotonJoinRoomFailed(object[] codeAndMsg)
    {
        // 0: 오류 코드, 1: 오류 내용
        Debug.Log("[ERROR] Room Connection Error:" + codeAndMsg[1].ToString());
        StartCoroutine(LoadMessageCoroutine(codeAndMsg[1].ToString(), 2f));
    }

    public void GetRoomLIst()
    {
        RoomInfo[] roomInfos = PhotonNetwork.GetRoomList();
        //Debug.Log(roomInfos.Length);

        for (int i = 0; i < roomInfos.Length; i++)
        {
            if (m_roomItemPrefab == null)
                return;
            if (m_roomListParent == null)
                return;

            GameObject roomItemgo = Instantiate(m_roomItemPrefab, m_roomListParent.transform);
            roomItemgo.name = m_roomItemPrefab.name + i + 1;

            RoomListItem item = roomItemgo.GetComponent<RoomListItem>();
            if (item == null)
            {
                Destroy(roomItemgo);
                return;
            }

            item.InitRoomListItem(roomInfos[i].Name, roomInfos[i].PlayerCount, roomInfos[i].MaxPlayers);

        }
    }

    public void RefreshRoomLIst()
    {
        if (m_roomListParent.transform.childCount > 1)
        {
            for (int i = 0; i < m_roomListParent.transform.childCount; i++)
            {
                Transform child = m_roomListParent.transform.GetChild(i);
                Destroy(child.gameObject);
            }
        }

        GetRoomLIst();
    }

    public void TurnCreateMenu(bool isTurn)
    {
        if (m_createMenugo != null)
            m_createMenugo.SetActive(isTurn);
    }

    public void TurnJoinMenu(bool isTurn)
    {
        if (m_joinMenugo == null)
            return;

        m_joinMenugo.SetActive(isTurn);

        if (m_joinMenugo.activeSelf)
        {
            GetRoomLIst();
        }
            
    }

    public void LoadMessageInTime(string message,float fTime)
    {
        StartCoroutine(LoadMessageCoroutine(message, fTime));
    }

    private IEnumerator LoadMessageCoroutine(string message,float fTime)
    {
        if (string.IsNullOrEmpty(message))
            yield break;

        if (m_lobbyMessage == null)
            yield break;

        string backMessage = m_lobbyMessage.text;
        m_lobbyMessage.text = message;

        yield return new WaitForSeconds(fTime);

        m_lobbyMessage.text = backMessage;
    }

}
