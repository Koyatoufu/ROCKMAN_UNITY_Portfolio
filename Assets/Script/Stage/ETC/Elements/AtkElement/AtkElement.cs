using UnityEngine;
using System.Collections;

public class AtkElement : ElementBase {

	protected float m_fSpeed;

	protected AtkElement()
	{
		m_fSpeed = 0.0f;
		m_bAllive = false;
	}

	public void SetSpeed(float fSpeed)
	{
		m_fSpeed = fSpeed;
	}
		
    protected virtual void DamageFunc(UnitBase unitBase)
    {
        if(MultyManager.Inst!=null)
        {
            if(unitBase.photonView!=null&&!unitBase.photonView.isMine)
            {
                unitBase.photonView.RPC("GetDamage", PhotonTargets.AllBufferedViaServer, m_nValue);
            }
        }
        else
        {
            unitBase.GetDamage(m_nValue);
        }
    }

    protected void DamageFuncMove(UnitBase unitBase,int nX)
    {
        if (MultyManager.Inst != null)
        {
            if (unitBase.photonView != null && !unitBase.photonView.isMine)
            {
                unitBase.photonView.RPC("GetDamage", PhotonTargets.AllBufferedViaServer, m_nValue);
                unitBase.photonView.RPC("PanelMoveBack", PhotonTargets.AllBufferedViaServer, nX);
            }
        }
        else
        {
            unitBase.GetDamage(m_nValue);
            unitBase.PanelMoveBack(nX);
        }
    }
}
