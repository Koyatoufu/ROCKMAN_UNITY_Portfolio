using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitBase : MonoBehaviour {
	

	protected E_ACT m_act;

	protected Panel m_CurPanel;
	protected Panel m_NextPanel;
	protected List<Panel> m_listMovePanel;

	protected UnitStatus m_status;
	protected UnitAtk m_baseAtk;

	protected Animator m_anim;
	protected string m_szOppoPanel;
	protected TextMesh m_textHp;

	protected Material m_bodyMaterial;
	protected AI m_AI;

	protected Transform m_transAtk;

	protected delegate void delModeChange();
	protected delModeChange[] m_arModeFunc;

	protected GameObject m_goExplosion;

	protected bool m_bLock;
	protected float m_fChargeTime;
	protected float m_fMaxChargeTime;
	protected bool m_bCharged;


	public UnitBase()
	{
		m_act = E_ACT.IDLE;
		m_CurPanel = null;
		m_NextPanel = null;
		m_status = new UnitStatus ();
		m_baseAtk = new UnitAtk ();
		m_listMovePanel = null;
		m_anim = null;
		m_szOppoPanel = null;
		m_textHp = null;
		m_bodyMaterial = null;
		m_AI = null;
		m_transAtk = null;
		m_goExplosion = null;

		m_bLock = false;
		m_fChargeTime = 0.0f;
		m_fMaxChargeTime = 0.0f;
		m_bCharged = false;

		m_arModeFunc = new delModeChange[4];
		m_arModeFunc [0] = ChangeMaterialOpaque;
		m_arModeFunc [1] = ChangeMaterialCutOut;
		m_arModeFunc [2] = ChangeMaterialFade;
		m_arModeFunc [3] = ChangeMaterialTransParent;
	}

	void Awake()
	{
		m_act = E_ACT.IDLE;
	}

	public virtual void GetDamage(int nDamage){
		
	}

	public E_ACT GetAct(){
		return m_act;
	}
	public void SetAct(E_ACT act)
	{
		m_act = act;
	}
	public void SetCurPanel(Panel panel){
		if(m_CurPanel!=null)
		{
			m_CurPanel.SetPassable (true);
		}
		m_CurPanel = panel;
		m_CurPanel.SetPassable (false);
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
	public string GetOppoPanel(){
		return m_szOppoPanel;
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

	public virtual IEnumerator AttackUnit (){
		yield return null;
	}

	public void SetAttackBaseData(UnitAtk unitAtk)
	{
		m_baseAtk = unitAtk;
	}

	public float GetMaxChargeTime()
	{
		return m_fMaxChargeTime;
	}

	public void SetMaxChargeTime(float fMaxChargeTime)
	{
		m_fMaxChargeTime = fMaxChargeTime;
	}

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

	protected IEnumerator ExplosionPool()
	{
		yield return new WaitForSeconds (2.0f);
		if(m_goExplosion!=null)
		{
			ObjectPool.GetInst ().PooledObject (m_goExplosion);
		}
		yield return null;
	}
}
