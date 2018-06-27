using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum ColorDef
{
    Blue,
    Red
}

public class PlayerUnit:UnitBase
{
    //protected UnitAtk[] m_Chips = new UnitAtk[30];

    #region Instance

    private GameObject m_goBuster;

	private SkinnedMeshRenderer m_BusterMesh;

	protected delegate IEnumerator delUseChip (ChipData chipData);
	protected delUseChip[] m_arChipFunc;

	private GameObject m_goWeaponArm = null;
	private GameObject m_goHand = null;
	private GameObject m_goChip = null;

	private GameObject m_goCharge = null;
	private GameObject m_goChargeMax = null;

	private GameObject m_goChargeAtk = null;

    #endregion

    #region Initailize

    protected override void Awake()
	{
        base.Awake();
        //m_Chips = new UnitAtk[30];

        m_status.szName = "Player";

        m_fMaxChargeTime = 2.8f;

        m_status.nMaxHp = 100;
		m_status.nCurHp = m_status.nMaxHp;
		m_status.fMoveSpeed = 5.0f;

		m_baseAtk.nDmg = 1;
		m_baseAtk.nID = 0;
		m_baseAtk.fSpeed = 1.0f;
		m_baseAtk.eType = (E_TYPE)0;
		m_baseAtk.objType=ElementMgr.GetInst ().GetElement ((int)E_ATKELEMENT.BULLET);

		this.m_anim = gameObject.GetComponent<Animator> ();
		m_bodyMaterial = Resources.Load<Material> ("Materials/Unit/Material1");
		m_goBuster=transform.Find ("WeaponArm").gameObject;
		m_BusterMesh = m_goBuster.GetComponent<SkinnedMeshRenderer>();
		InitializeDelegate ();

		m_goCharge = transform.Find ("Charge").gameObject;
		m_goChargeMax = transform.Find ("ChargeMax").gameObject;
		m_goChargeAtk = ElementMgr.GetInst ().GetElement ((int)E_ATKELEMENT.CHARGEBULLET);
	}

	// Use this for initialization
	protected void Start ()
    {
		m_anim.SetInteger ("CurAnim", (int)E_PlAnimState.IDLE);
		m_anim.speed = 0.0f;
		m_transAtk = this.transform.Find ("AtkPos").transform;
		UIMgr.Inst.GetHpText ().text = m_status.nCurHp.ToString();

		FindArmBone();
		FindHandBone();

		m_goExplosion = EffectMgr.GetInst ().GetEffect ((int)E_EFFECTID.DEATH);

		m_bodyMaterial.color = Color.white;
		m_arModeFunc [0]();
	}

    #endregion

    protected void Update()
	{
		if (StageMgr.Inst==null||!StageMgr.Inst.IsPlay)
			return;

		m_fChargeTime += Time.deltaTime * 0.3f;
		if (m_fChargeTime >= m_fMaxChargeTime) {

			m_fChargeTime = m_fMaxChargeTime;
			m_bCharged = true;
		} 

		if (m_act != E_ACT.IDLE) {
			m_goCharge.SetActive (false);
			m_goChargeMax.SetActive (false);
			return;
		} 

		if (m_bCharged) {
			m_goCharge.SetActive (false);
			m_goChargeMax.SetActive (true);
			return;
		} 

		m_goCharge.SetActive (true);
		m_goChargeMax.SetActive (false);
	}

	protected override IEnumerator AttackCorutine ()
	{
		if(m_goChip!=null)
		{
			yield break;
		}

		while(m_act==E_ACT.ATK)
		{
			if(StageMgr.Inst.IsPlay==false)
			{
				m_act = E_ACT.IDLE;
				yield break;
			}
				
			int nPlAnim = (int)E_PlAnimState.NormalShot;
			int nDmg = m_baseAtk.nDmg;
			GameObject objType = m_baseAtk.objType;
			if(m_bCharged==true)
			{
				m_bCharged = false;
				nPlAnim = (int)E_PlAnimState.RecoilShot;
				objType=m_goChargeAtk;
				nDmg *= 10;
			}
			m_anim.SetInteger ("CurAnim",nPlAnim);
			m_fChargeTime = 0.0f;
			yield return null;
			GameObject objTmp = ObjectPool.GetInst().GetObject(objType);

			AtkElement atkTmp = objTmp.GetComponent<AtkElement> ();
			atkTmp.SetValue (nDmg);

			atkTmp.SetUnit(this);
			objTmp.transform.position = m_transAtk.position;
			yield return new WaitForSeconds(0.5f);
		}
	}

    public override void GetDamage(int nDamage)
	{
        if (m_act == E_ACT.DIE)
        {
            HpUIChange();
            return;
        }   

        DamageFunc(nDamage);
        HpUIChange();
        if (CheckDeath())
            return;
        HitFunc();
	}

    protected void DamageFunc(int nDamage)
    {
        m_status.nCurHp -= nDamage;
        m_fChargeTime = 0.0f;
        m_bCharged = false;
        StopCoroutine(AttackCorutine());
        StopCoroutine(CamMgr.GetInst().SubCamActive(this.transform));
        StopCoroutine(UseChipCoroutine(0));

        CamMgr.GetInst().GetSubCam().SetActive(false);
        if (m_goChip != null)
        {
            ObjectPool.GetInst().PooledObject(m_goChip);
            m_goChip = null;
        }
    }

    protected virtual void HpUIChange()
    {
        if (m_status.nCurHp < 0)
        {
            m_status.nCurHp = 0;
        }
        else if (m_status.nCurHp > m_status.nMaxHp)
        {
            m_status.nCurHp = m_status.nMaxHp;
        }

        UIMgr.Inst.GetHpText().text = m_status.nCurHp.ToString();
    }

    protected bool CheckDeath()
    {
        if (m_act == E_ACT.DIE)
            return true;

        if (m_status.nCurHp <= 0)
        {
            UIMgr.Inst.GetHpText().text = 0.ToString();
            m_act = E_ACT.DIE;
            m_anim.SetInteger("CurAnim", (int)E_PlAnimState.FallDown);
            return true;
        }

        return false;
    }

    protected virtual void HitFunc()
    {
        m_act = E_ACT.HIT;
        m_anim.SetInteger("CurAnim", (int)E_PlAnimState.Hit);
    }

    public virtual void HitAlphaChange()
    {
        StartCoroutine(ChangeBodysAlphaCoroutine());
    }
    
    public virtual void UseChip(int nIndex)
    {
        if (m_act != E_ACT.IDLE)
            return;

        StartCoroutine(UseChipCoroutine(nIndex));
    }

    protected virtual IEnumerator UseChipCoroutine(int nIndex){
		if(m_act!=E_ACT.IDLE)
			yield break;
		m_fChargeTime = 0.0f;
		ChipData ChipTmp=ChipMgr.Inst.GetUseChipUse (nIndex);
        
        ChipMgr.Inst.RemoveUseChipInUseDeck(nIndex);
		ChipMgr.Inst.ResetUseChip ();

		yield return null;

        m_anim.SetInteger ("CurAnim", ChipTmp.nAnimIndex);
		m_act=E_ACT.ATK;
		StartCoroutine (m_arChipFunc [(int)ChipTmp.eChipType] (ChipTmp));
		yield return null;
	}

    public virtual void UseShield()
    {
        if (m_act != E_ACT.IDLE && m_act!=E_ACT.ATK)
            return;

        StartCoroutine(UseShieldCoroutine());
    }

    protected IEnumerator UseShieldCoroutine()
	{
		m_fChargeTime = 0.0f;
		m_bCharged = false;
		m_act = E_ACT.GUARD;
		GameObject objShield=ObjectPool.GetInst().GetObject(EffectMgr.GetInst ().GetEffect ((int)E_EFFECTID.SHIELD));
		objShield.transform.parent = this.transform;
		objShield.transform.position = m_transAtk.transform.position+transform.forward*0.2f-new Vector3(0.0f,0.2f,0.0f);
		yield return new WaitForSeconds(1.0f);
		objShield.transform.parent = null;
		ObjectPool.GetInst ().PooledObject (objShield);
		yield return null;
		ResetIdle ();
		yield return null;
	}

	public virtual void ResetIdle()
	{
		m_act = E_ACT.IDLE;
		m_anim.SetInteger ("CurAnim", (int)E_PlAnimState.IDLE);
		StopCoroutine (ChangeBodysAlphaCoroutine ());
	}

	public virtual void PooledThis()
	{
		m_goExplosion=ObjectPool.GetInst().GetObject(m_goExplosion);
		m_goExplosion.transform.position = this.transform.position;
		m_CurPanel.Passable = true;
		StartCoroutine (ExplosionPool());
        ObjectPool.GetInst().PooledObject(this.gameObject);
    }

    private IEnumerator ChangeBodysAlphaCoroutine()
	{
		m_arModeFunc [3]();

        Color color = m_bodyMaterial.color;

        for (float f=0.0f;f<0.5f;f+=Time.deltaTime)
		{
            color.a = 0f;
			m_bodyMaterial.color = color;
			yield return null;
            color.a = 1f;
			m_bodyMaterial.color = color;
			yield return null;
		}

        color.a = 1f;
        m_bodyMaterial.color = color;

		m_arModeFunc [0]();

		yield return null;
	}
    
    public virtual void MultyPlayChangeBodyColor(){}

    private void InitializeDelegate()
	{
		m_arChipFunc = new delUseChip[(int)E_CHIPTYPE.MAX];

		m_arChipFunc [(int)E_CHIPTYPE.WEAPON] = UseWeaponCoroutine;
		m_arChipFunc [(int)E_CHIPTYPE.ATKEFFECT] = UseAtkEffectCoroutine;
		m_arChipFunc [(int)E_CHIPTYPE.SUMMON] = UseSummon;
		m_arChipFunc [(int)E_CHIPTYPE.RECOVERY] = UseRecovery;
		m_arChipFunc [(int)E_CHIPTYPE.ATTACH] = UseAttach;
		m_arChipFunc [(int)E_CHIPTYPE.AREA] = UseArea;
		m_arChipFunc [(int)E_CHIPTYPE.SUPPORT] = UseSupport;
		m_arChipFunc [(int)E_CHIPTYPE.NONE] = UseNone;
	}

    #region ChipCoroutine

    protected IEnumerator UseWeaponCoroutine(ChipData chipData)
	{
        UseSubCam();

		yield return null;

		GameObject goWeapon = null;

		if(chipData.objType==null)
		{
			yield break;
		}

		m_goChip=ObjectPool.GetInst ().GetObject (chipData.objType);

		goWeapon = m_goChip.transform.Find ("WeaponArm").gameObject;


		m_goChip.transform.parent = m_goWeaponArm.transform;
		//m_goChip.transform.localRotation = m_goHand.transform.localRotation;
		m_goChip.transform.localPosition = new Vector3(0.0f,0.0f,0.0f);
		m_goChip.transform.localRotation = Quaternion.identity;
		m_goChip.transform.Rotate (-90.0f,0.0f,0.0f);
		goWeapon.transform.localPosition = new Vector3(0.0f,0.0f,0.0f);
		goWeapon.transform.localRotation = Quaternion.identity;

		AtkElement atkTmp = goWeapon.transform.GetChild(0).GetComponent<AtkElement>();

		if(atkTmp!=null)
		{
			atkTmp.SetUnit (this);
			atkTmp.transform.localPosition = m_transAtk.position;
			atkTmp.SetValue(chipData.nValue);
		}

		yield return new WaitForSeconds(0.8f);

		m_goChip = null;
		ResetIdle ();
		yield return null;
	}

    protected virtual void UseSubCam()
    {
        StartCoroutine(CamMgr.GetInst().SubCamActive(transform));
    }

    private IEnumerator UseAtkEffectCoroutine(ChipData chipData)
	{
		yield return new WaitForSeconds (0.35f);
		m_goChip=ObjectPool.GetInst ().GetObject (chipData.objType);

		m_goChip.transform.position = m_goHand.transform.position;

		AtkElement atkTmp = m_goChip.transform.GetComponent<AtkElement> ();
		atkTmp.SetUnit (this);
		atkTmp.SetValue (chipData.nValue);

		yield return null;

		m_goChip = null;
		ResetIdle ();

		yield return null;
	}

    private IEnumerator UseSummon(ChipData chipData)
	{
		m_goChip=ObjectPool.GetInst ().GetObject (chipData.objType);
		yield return null;
		ResetIdle ();
		yield return null;
	}

    private IEnumerator UseSupport(ChipData chipData)
	{
		m_goChip=ObjectPool.GetInst ().GetObject (chipData.objType);
		m_goChip.transform.parent = this.transform;
		m_goChip.transform.position = transform.position;

		SupportElement eletmp = m_goChip.GetComponent<SupportElement> ();
		if(eletmp!=null)
		{
			eletmp.SetValue (chipData.nValue);
			eletmp.SetUnit (this);
		}
	
		yield return null;

		m_goChip = null;

		ResetIdle ();

		yield return null;

	}

    private IEnumerator UseAttach(ChipData chipData)
	{
		m_goChip=ObjectPool.GetInst ().GetObject (chipData.objType);
		ChipData chipTmp=ChipMgr.Inst.GetUseChip (0);
        
		if(chipTmp.eChipType!=E_CHIPTYPE.ATKEFFECT
            &&chipTmp.eChipType!=E_CHIPTYPE.WEAPON
            &&chipTmp.eChipType!=E_CHIPTYPE.SUMMON
            &&chipTmp.eChipType!=E_CHIPTYPE.ATTACH)
		{
			StartCoroutine(PoofFunc ());
			yield break;
		}

		chipTmp.nValue += chipData.nValue;
		ChipMgr.Inst.SetUseChip (0,chipTmp);
		chipTmp = ChipMgr.Inst.GetUseChip (0);
		Debug.Log (chipTmp.nValue);

		yield return null;

		ObjectPool.GetInst ().PooledObject (m_goChip);
		m_goChip = null;
		ResetIdle ();
		ChipMgr.Inst.ResetUseChip ();

		yield return null;
	}

    protected virtual IEnumerator UseArea(ChipData chipData)
	{
		m_goChip=ObjectPool.GetInst ().GetObject (chipData.objType);
		yield return null;

		AreaElement areaTmp = m_goChip.GetComponent<AreaElement> ();
		areaTmp.SetUnit (this);
		areaTmp.SetValue (chipData.nValue);
		yield return null;

		m_goChip = null;
		ResetIdle ();

		yield return null;
	}

    private IEnumerator UseRecovery(ChipData chipData)
	{
		m_goChip=ObjectPool.GetInst ().GetObject (chipData.objType);

		m_goChip.transform.position = transform.position+new Vector3(0.0f,1.5f,0.0f);

		if(m_status.nCurHp<m_status.nMaxHp&&m_act==E_ACT.DIE)
		{
			ObjectPool.GetInst ().PooledObject (m_goChip);
			m_goChip = null;
			yield break;
		}

        
		m_status.nCurHp += chipData.nValue;

        HpUIChange();

		yield return new WaitForSeconds(0.65f);

		ObjectPool.GetInst ().PooledObject (m_goChip);
		m_goChip = null;
		ResetIdle ();

		yield return null;
	}
	private IEnumerator UseNone(ChipData chipData)
	{
		yield return null;
	}

    private IEnumerator PoofFunc()
	{
		ObjectPool.GetInst ().PooledObject (m_goChip);
		m_goChip = null;
		yield return null;
		GameObject goPoof=ObjectPool.GetInst ().GetObject (EffectMgr.GetInst().GetEffect((int)E_EFFECTID.POOF));
		goPoof.transform.position = transform.position + new Vector3 (0.0f,3.5f,0.0f);
		ResetIdle ();
		yield return new WaitForSeconds (0.3f);
		ObjectPool.GetInst ().PooledObject (goPoof);
		yield return null;
	}

    #endregion

    private void FindArmBone()
	{
		string szName="R_LowArm";
		for(int i=0;i<m_BusterMesh.bones.Length;i++)
		{
			if(m_BusterMesh.bones[i].name.CompareTo(szName)==0)
			{
				m_goWeaponArm = m_BusterMesh.bones [i].gameObject;
				break;
			}
		}
	}

	private void FindHandBone()
	{
		string szName="R_Hand";
		for(int i=0;i<m_BusterMesh.bones.Length;i++)
		{
			if(m_BusterMesh.bones[i].name.CompareTo(szName)==0)
			{
				m_goHand = m_BusterMesh.bones [i].gameObject;
				break;
			}
		}
	}
}
