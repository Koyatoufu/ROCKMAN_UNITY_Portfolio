using UnityEngine;
using System.Collections;

public class Throw : AtkElement
{

    private float m_fStartTime = 0f;

    private GameObject m_goExplosion = null;

    void Awake()
    {
        m_fSpeed = 1.0f;
        m_fStartTime = 0.0f;
    }

    void OnEnable()
    {
        m_bAllive = true;
        StartCoroutine(ExecuteCoroutine());
        m_fStartTime = Time.time;
    }
    void OnDisable()
    {
        m_bAllive = false;
        m_Unit = null;
        m_fStartTime = 0.0f;
    }

    void OnTriggerEnter(Collider collider)
    {
        UnitBase unitBase = collider.GetComponent<UnitBase>();
        if (unitBase != null)
        {
            if (unitBase == m_Unit)
                return;

            DamageFunc(unitBase);
        }
        ElementBase eleTmp = collider.GetComponent<ElementBase>();
        if (eleTmp != null)
        {
            if (eleTmp.GetUnit().GetType() == m_Unit.GetType())
                return;
        }

        m_goExplosion = ObjectPool.GetInst().GetObject(EffectMgr.GetInst().GetEffect((int)E_EFFECTID.EXPLOSION));

        m_goExplosion.transform.position = transform.position;

        PooledThis();
    }

    protected override IEnumerator ExecuteCoroutine()
    {
        yield return new WaitUntil(() => m_Unit != null);
        int nXForward = m_Unit.transform.forward.x < 0 ? -3 : 3;
        Vector3 vTarget = MapMgr.Inst.GetMapPanel(m_Unit.GetCurPanel().GetPoint().nX + nXForward, m_Unit.GetCurPanel().GetPoint().nZ).transform.position;
        while (m_bAllive)
        {
            Vector3 vCenter = (transform.position + vTarget) * 0.5f;
            vCenter.y -= 1.5f;
            Vector3 vRelCenter = transform.position - vCenter;
            Vector3 vTargetRelCenter = vTarget - vCenter;
            float fFraccomplete = (Time.time - m_fStartTime) * 0.125f;
            transform.position = Vector3.Slerp(vRelCenter, vTargetRelCenter, fFraccomplete);
            transform.position += vCenter;
            transform.Rotate(new Vector3(0.0f, 1.0f, 0.0f), Time.deltaTime * 5.0f);
            yield return null;
        }
    }

    public override void PooledThis()
    {
        StageMgr.Inst.StartCoroutine(PooledExplosion());
        base.PooledThis();
    }

    private IEnumerator PooledExplosion()
    {
        yield return new WaitForSeconds(0.5f);
        GameObject objTmp = m_goExplosion;
        m_goExplosion = null;
        ObjectPool.GetInst().PooledObject(objTmp);
        yield return null;
    }
}
