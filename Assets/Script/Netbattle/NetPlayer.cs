using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetPlayer : PlayerUnit
{
    [SerializeField]
    private SkinnedMeshRenderer[] m_colorMeshs = null;
    [SerializeField]
    private Material[] m_colorMat = null;

    protected override void Awake()
    {
        base.Awake();

        m_status.nMaxHp = 200;
        m_status.nCurHp = m_status.nMaxHp;
    }

    [PunRPC]
    public override void GetDamage(int nDamage)
    {
        base.GetDamage(nDamage);
    }

    protected override void HpUIChange()
    {
        if (photonView.isMine)
        {
            Debug.Log(photonView.viewID);
            base.HpUIChange();
        }

        photonView.RPC("SetHpText", PhotonTargets.AllBufferedViaServer);
    }

    [PunRPC]
    public override void AttackUnit()
    {
        base.AttackUnit();
    }

    [PunRPC]
    public override void StopAttack()
    {
        base.StopAttack();
    }

    [PunRPC]
    private void SetHpText()
    {
        m_textHp.text = m_status.nCurHp.ToString();

        if (m_status.nCurHp < 0)
            m_textHp.text = 0.ToString();
    }

    public override void ResetIdle()
    {
        if (photonView.isMine)
            photonView.RPC("IdleRpc", PhotonTargets.AllBufferedViaServer);
    }

    public override void HitAlphaChange()
    {
        if(photonView.isMine)
            photonView.RPC("HitRpc", PhotonTargets.AllBufferedViaServer);
    }

    [PunRPC]
    private void IdleRpc()
    {
        base.ResetIdle();
    }

    [PunRPC]
    private void HitRpc()
    {
        base.HitAlphaChange();
    }

    public override void PooledThis()
    {
        photonView.RPC("PooledRpc", PhotonTargets.AllBufferedViaServer);
    }

    [PunRPC]
    private void PooledRpc()
    {
        m_goExplosion = ObjectPool.GetInst().GetObject(m_goExplosion);
        m_goExplosion.transform.position = transform.position;
        if(m_CurPanel!=null)
            m_CurPanel.Passable = true;
        StageMgr.Inst.StartCoroutine(ExplosionPool());

        if (UnitMgr.Inst.Player == this)
            UnitMgr.Inst.Player = null;
        else if (UnitMgr.Inst.OtherPlayer == this)
            UnitMgr.Inst.OtherPlayer = null;

        //TODO: 사망 상태 전달
        UnitMgr.Inst.CheckNetUnitDie(this);
        gameObject.SetActive(false);
    }

    [PunRPC]
    public override void MultyPlayChangeBodyColor()
    {
        Debug.Log("ColorChange");

        if (m_colorMat.Length<1)
            return;

        Material mat = null;
        mat = !IsRed? m_colorMat[0] : m_colorMat[1];

        for (int i=0;i<m_colorMeshs.Length;i++)
        {
            m_colorMeshs[i].material = mat;
        }

        m_bodyMaterial = mat;
    }

    [PunRPC]
    public override void UseChip(int nIndex)
    {
        base.UseChip(nIndex);
    }

    protected override IEnumerator UseChipCoroutine(int nIndex)
    {
        if (m_act != E_ACT.IDLE)
            yield break;
        m_fChargeTime = 0.0f;

        if (photonView.isMine)
        {
            ChipData chipData = ChipMgr.Inst.GetUseChipUse(nIndex);

            ChipMgr.Inst.RemoveUseChipInUseDeck(nIndex);
            ChipMgr.Inst.ResetUseChip();

            photonView.RPC("ChipFuncRpc",PhotonTargets.AllBufferedViaServer, chipData.nID);

            yield return null;
        }
        
        yield return null;
    }

    [PunRPC]
    private void ChipFuncRpc(int chipId)
    {
        ChipData chipData;

        DBMgr.GetInst().SelectStandard(chipId, out chipData);

        m_anim.SetInteger("CurAnim", chipData.nAnimIndex);
        m_act = E_ACT.ATK;
        StartCoroutine(m_arChipFunc[(int)chipData.eChipType](chipData));
    }

    [PunRPC]
    public override void UseShield()
    {
        if(photonView.isMine)
            base.UseShield();
    }

    [PunRPC]
    public void NetInit()
    {
        m_anim.speed = 1f;
        m_textHp.text = m_status.nCurHp.ToString();
        IsHoldOn = true;

        int nXPenel = IsRed ? 1 : -2;
        SetCurPanel(MapMgr.Inst.GetMapPanel(nXPenel, 0));
        transform.position = GetCurPanel().transform.position;

        photonView.RPC("MultyPlayChangeBodyColor", PhotonTargets.AllBufferedViaServer);
    }

    public override void CustomHoldOnStart()
    {
        if (photonView != null && photonView.owner == PhotonNetwork.player)
            photonView.RPC("CustomHoldStartRpc", PhotonTargets.AllBufferedViaServer);
    }

    [PunRPC]
    private void CustomHoldStartRpc()
    {
        base.CustomHoldOnStart();
    }

    public override void CustomHoldOnEnd()
    {
        if (photonView != null && photonView.owner==PhotonNetwork.player)
            photonView.RPC("CustomHoldEndRpc", PhotonTargets.AllBufferedViaServer);
    }

    [PunRPC]
    private void CustomHoldEndRpc()
    {
        base.CustomHoldOnEnd();
    }

    [PunRPC]
    public override void PanelMoveBack(int nX)
    {
        base.PanelMoveBack(nX);
    }

    [PunRPC]
    public void ChangePanelColor(int nX, int nZ)
    {
        Panel pCol = MapMgr.Inst.GetMapPanel(nX, nZ);

        if (pCol == null)
            return;

        Debug.Log(pCol);

        pCol.ReversePanel();
    }

    protected override void UseSubCam()
    {
        if(photonView!=null&&photonView.owner==PhotonNetwork.player)
            base.UseSubCam();
    }

    [PunRPC]
    private void MovePanel(int nX,int nZ)
    {
        if (MapMgr.Inst == null)
            return;

        Panel pDest = MapMgr.Inst.GetMapPanel(nX, nZ);

        if (pDest == null)
            return;

        SetCurPanel(pDest);
        transform.position = pDest.transform.position;
    }

    /*
    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(gameObject.activeSelf);
        }
        else
        {
            gameObject.SetActive((GameObject)stream.ReceiveNext());
        }
    }
    */
}
