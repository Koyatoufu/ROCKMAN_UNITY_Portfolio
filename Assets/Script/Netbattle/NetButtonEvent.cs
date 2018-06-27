using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetButtonEvent : ButtonEvent
{
    public override void AtackPlayer()
    {
        if (StageMgr.Inst.IsPlay == false)
        {
            return;
        }

        PlayerUnit player = UnitMgr.Inst.Player as PlayerUnit;

        if (player != null)
        {
            if (MultyManager.Inst != null)
            {
                if (player.photonView != null && player.photonView.isMine)
                {
                    player.photonView.RPC("AttackUnit", PhotonTargets.AllBufferedViaServer);
                }
            }
        }
    }

    public override void StopAttackPlayer()
    {
        PlayerUnit player = UnitMgr.Inst.Player as PlayerUnit;

        if (player == null)
            return;

        if (MultyManager.Inst != null)
        {
            if (player.photonView != null && player.photonView.isMine)
            {
                player.photonView.RPC("StopAttack", PhotonTargets.AllBufferedViaServer);
            }
        }
    }

    public override void GaugeBtn()
    {
        if (StageMgr.Inst.IsPlay == false)
        {
            return;
        }
        if (UIMgr.Inst.GetGaugeAnim().GetBool("Full"))
        {
            UIMgr.Inst.SetActiveCustom();
            UIMgr.Inst.SetActiveMain();

            ChipMgr.Inst.ClearUseChipData();
            ChipMgr.Inst.StartCustomChipSet();

            if (UnitMgr.Inst.Player != null)
            {
                UnitMgr.Inst.Player.CustomHoldOnStart();
            }

            StageMgr.Inst.photonView.RPC("TimeScaleChange", PhotonTargets.AllBufferedViaServer, true);
        }
    }

    public override void PauseGame(bool isPause)
    {
        UIMgr.Inst.SetActivePause();
        if (UIMgr.Inst.GetCustomed() == false)
        {
            if (StageMgr.Inst.photonView != null)
                StageMgr.Inst.photonView.RPC("TimeScaleChange", PhotonTargets.AllBufferedViaServer,isPause);
            else
                StageMgr.Inst.TimeScaleChange(isPause);
        }
    }

    public override void UseChip(int nIndex)
    {
        if (StageMgr.Inst.IsPlay == false)
        {
            return;
        }

        PlayerUnit player = UnitMgr.Inst.Player as PlayerUnit;

        if (player == null)
            return;

        if (player.photonView != null && player.photonView.isMine)
            player.photonView.RPC("UseChip", PhotonTargets.AllBufferedViaServer, nIndex);
    }

    public override void UseShield()
    {
        PlayerUnit player = UnitMgr.Inst.Player as PlayerUnit;

        if (player == null)
            return;

        if(player.photonView!=null && player.photonView.isMine)
            player.photonView.RPC("UseShield", PhotonTargets.AllBufferedViaServer);
    }

    public override void ReturnTitle()
    {
        PauseGame(false);
        base.ReturnTitle();
    }
}
