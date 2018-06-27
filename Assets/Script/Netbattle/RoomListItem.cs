using UnityEngine;
using UnityEngine.UI;

public class RoomListItem : MonoBehaviour
{
    private string m_roomName = null;
    public string RoomName { get { return m_roomName; }}

    public int PlayerCount { get; private set; }
    public int PlayerLimit { get; private set; }

    [SerializeField]
    private Text m_infoText = null;

    public void InitRoomListItem(string name, int count, int limit)
    {
        if (string.IsNullOrEmpty(name))
            return;
        if (limit < 1)
            return;

        if (!string.IsNullOrEmpty(m_roomName))
            return;

        m_roomName = name;
        PlayerCount = count;
        PlayerLimit = limit;

        if(m_infoText!=null)
            m_infoText.text = name + " - " + PlayerCount + "/" + PlayerLimit;
    }

    public void JoinRoom()
    {
        if (LobbyMgr.Inst == null)
            return;

        if (string.IsNullOrEmpty(m_roomName))
            return;

        PhotonNetwork.playerName = "Player" + PlayerCount+1;
        LobbyMgr.Inst.CreateOrJoinRoom(RoomName);
    }
}
