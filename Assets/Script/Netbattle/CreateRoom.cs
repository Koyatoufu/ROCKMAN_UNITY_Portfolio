using UnityEngine;

public class CreateRoom : MonoBehaviour
{
    private string m_roomName = "";

    public void SetRoomName(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return;
        }
            
        m_roomName = name;
    }

    public void CreateRoomFunc()
    {
        if (LobbyMgr.Inst == null)
            return;
        if (string.IsNullOrEmpty(m_roomName))
        {
            LobbyMgr.Inst.LoadMessageInTime("RoomName is Empty", 2f);
            return;
        }
            
        PhotonNetwork.playerName = "Player1";

        LobbyMgr.Inst.CreateOrJoinRoom(m_roomName);
    }
}
