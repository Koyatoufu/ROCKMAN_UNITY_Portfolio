using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitBase : Photon.MonoBehaviour
{
	protected E_ACT m_act = E_ACT.IDLE;

	protected Panel m_CurPanel = null;
	protected Panel m_NextPanel = null;
	protected List<Panel> m_listMovePanel = null;

    [SerializeField]
    protected TextMesh m_textHp = null;
    [SerializeField]
    protected Material m_bodyMaterial = null;

    protected GameObject m_goExplosion = null;

    protected UnitStatus m_status;
	protected UnitAtk m_baseAtk;

	protected Animator m_anim = null;
    protected AI m_AI = null;

	protected Transform m_transAtk = null;

	protected delegate void delModeChange();
	protected delModeChange[] m_arModeFunc;

	protected float m_fChargeTime = 0f;
	protected float m_fMaxChargeTime = 0f;
    public float MaxChargeTime { get { return m_fMaxChargeTime; } set { m_fMaxChargeTime = value; } }

	protected bool m_bCharged = false;

    public bool IsRed { get; set; }

    public bool IsHoldOn { get; protected set; }

    protected virtual void Awake()
	{
		m_act = E_ACT.IDLE;

        m_arModeFunc = new delModeChange[4];
        m_arModeFunc[0] = ChangeMaterialOpaque;
        m_arModeFunc[1] = ChangeMaterialCutOut;
        m_arModeFunc[2] = ChangeMaterialFade;
        m_arModeFunc[3] = ChangeMaterialTransParent;
    }

    #region GetSet
    public E_ACT GetAct(){
		return m_act;
	}
	public void SetAct(E_ACT act)
	{
		m_act = act;
	}
	public void SetCurPanel(Panel panel){
        if (panel == null)
            return;

		if(m_CurPanel!=null)
		{
			m_CurPanel.Passable = true;
		}
		m_CurPanel = panel;
		m_CurPanel.Passable = false;
	}
	public Panel GetCurPanel()
	{
		return m_CurPanel;
	}
	public List<Panel> GetMovePanels(){
		return m_listMovePanel;
	}
	public void SetMovePanels(List<Panel> panels){
		m_listMovePanel = panels;
	}
	public Animator GetAnim()
	{
		return m_anim;
	}
	public AI GetAI()
	{
		return m_AI;
	}
	public UnitStatus GetStatus()
	{
		return m_status;
	}
	public UnitAtk GetAttackBase()
	{
		return m_baseAtk;
	}
	public Transform GetTransformAtk()
	{
		return m_transAtk;
	}
    public void SetAttackBaseData(UnitAtk unitAtk)
    {
        m_baseAtk = unitAtk;
    }
    #endregion

    public virtual void GetDamage(int nDamage)
    {

    }

    public virtual void AttackUnit()
    {
        if (m_act != E_ACT.IDLE)
        {
            Debug.Log(m_act);
            return;
        }
            

        SetAct(E_ACT.ATK);
        StartCoroutine(AttackCorutine());
    }

	protected virtual IEnumerator AttackCorutine (){
		yield return null;
	}

    public virtual void StopAttack()
    {
        SetAct(E_ACT.IDLE);
        m_anim.SetInteger("CurAnim", (int)E_PlAnimState.IDLE);
        StopCoroutine(AttackCorutine());
    }

    public virtual void CustomHoldOnStart()
    {
        IsHoldOn = true;
    }

    public virtual void CustomHoldOnEnd()
    {
        IsHoldOn = false;
    }

    #region MaterialMode

    protected void ChangeMaterialOpaque()
	{
		m_bodyMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
		m_bodyMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
		m_bodyMaterial.SetInt("_ZWrite", 1);
		m_bodyMaterial.DisableKeyword("_ALPHATEST_ON");
		m_bodyMaterial.DisableKeyword("_ALPHABLEND_ON");
		m_bodyMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
		m_bodyMaterial.renderQueue = -1;
	}
	protected void ChangeMaterialCutOut()
	{
		m_bodyMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
		m_bodyMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
		m_bodyMaterial.SetInt("_ZWrite", 1);
		m_bodyMaterial.EnableKeyword("_ALPHATEST_ON");
		m_bodyMaterial.DisableKeyword("_ALPHABLEND_ON");
		m_bodyMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
		m_bodyMaterial.renderQueue = 2450;
	}
	protected void ChangeMaterialFade()
	{
		m_bodyMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
		m_bodyMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
		m_bodyMaterial.SetInt("_ZWrite", 0);
		m_bodyMaterial.DisableKeyword("_ALPHATEST_ON");
		m_bodyMaterial.EnableKeyword("_ALPHABLEND_ON");
		m_bodyMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
		m_bodyMaterial.renderQueue = 3000;
	}
	protected void ChangeMaterialTransParent()
	{
		m_bodyMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
		m_bodyMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
		m_bodyMaterial.SetInt("_ZWrite", 0);
		m_bodyMaterial.DisableKeyword("_ALPHATEST_ON");
		m_bodyMaterial.EnableKeyword("_ALPHABLEND_ON");
		m_bodyMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
		m_bodyMaterial.renderQueue = 3000;
	}

    #endregion

    protected IEnumerator ExplosionPool()
	{
		yield return new WaitForSeconds (2.0f);
		if(m_goExplosion!=null)
		{
			ObjectPool.GetInst ().PooledObject (m_goExplosion);
		}
		yield return null;
	}

    public virtual void PanelMoveBack(int nX)
    {
        if (GetCurPanel() != null)
        {
            Panel pDest = MapMgr.Inst.GetMapPanel(GetCurPanel().GetPoint().nX + nX, GetCurPanel().GetPoint().nZ);
            UnitMgr.Inst.MoveUnit(this, GetCurPanel(), pDest);
        }
    }
}
