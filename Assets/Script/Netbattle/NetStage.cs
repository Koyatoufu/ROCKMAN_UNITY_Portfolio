using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetStage : StageMgr
{
    [PunRPC]
    public override void ReleaseStage()
    {
        //if(PhotonNetwork.isMasterClient)
        base.ReleaseStage();

        if (PhotonNetwork.inRoom)
        {
            PhotonNetwork.LeaveRoom();
            PhotonNetwork.LoadLevel("MainScene");
            if(PhotonNetwork.room.PlayerCount<1 && PhotonNetwork.connecting)
                PhotonNetwork.Disconnect();
        }
        else
        {
            PhotonNetwork.LoadLevel("MainScene");
        }
    }

    [PunRPC]
    public override void TimeScaleChange(bool isPause)
    {
        base.TimeScaleChange(isPause);

        if (PhotonNetwork.room == null || PhotonNetwork.room.PlayerCount < PhotonNetwork.room.MaxPlayers)
            return;

        if(isPause)
            MultyManager.Inst.ShowMessage("Hold on Other");
        else
            MultyManager.Inst.HideMessage();
    }

    public override void ConfirmChips()
    {
        if (UIMgr.Inst == null || ChipMgr.Inst == null || UnitMgr.Inst==null)
            return;

        UIMgr.Inst.SetActiveCustom();
        UIMgr.Inst.SetActiveMain();
        UIMgr.Inst.SetCustomConfirm();

        UIMgr.Inst.GetGaugeImage().fillAmount = 0.0f;

        ChipMgr.Inst.ConfirmUseChips();

        if(UnitMgr.Inst.Player!=null)
            UnitMgr.Inst.Player.CustomHoldOnEnd();

        if(UnitMgr.Inst.OtherPlayer!=null)
            photonView.RPC("TimeScaleChange", PhotonTargets.AllBufferedViaServer, UnitMgr.Inst.OtherPlayer.IsHoldOn);
    }

    protected override IEnumerator GameOverCoroutine(int nStateAnim)
    {
        return base.GameOverCoroutine(1);
    }

    protected override IEnumerator ClearStageCoroutine(int nStateAnim)
    {
        return base.ClearStageCoroutine(1);
    }
}