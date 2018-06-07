using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerUnit:UnitBase
{
	protected UnitAtk[] m_Chips=null;

	private GameObject m_goBuster;
	private SkinnedMeshRenderer m_BusterMesh;

	private delegate IEnumerator delUseChip (ChipData chipData);
	private delUseChip[] m_arChipFunc;

	private GameObject m_goWeaponArm;
	private GameObject m_goHand;
	private GameObject m_goChip;

	private GameObject m_goCharge;
	private GameObject m_goChargeMax;

	private GameObject m_goChargeAtk;

	PlayerUnit()
	{
		m_status.szName = "Player";
		m_szOppoPanel = "EnemyArea";
		m_Chips = new UnitAtk[30];

		m_fMaxChargeTime = 2.8f;

		m_goBuster = null;
		m_arChipFunc = null;
		m_goHand = null;
		m_goChip = null;
		m_goWeaponArm = null;

		m_goCharge = null;
		m_goChargeMax = null;
		m_goChargeAtk = null;
	}

	void Awake()
	{
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
		m_goBuster=transform.Find("WeaponArm").gameObject;
		m_BusterMesh = m_goBuster.GetComponent<SkinnedMeshRenderer>();
		InitializeDelegate ();

		m_goCharge = transform.Find("Charge").gameObject;
		m_goChargeMax = transform.Find("ChargeMax").gameObject;
		m_goChargeAtk = ElementMgr.GetInst ().GetElement ((int)E_ATKELEMENT.CHARGEBULLET);
	}

	// Use this for initialization
	void Start () {
		m_anim.SetInteger ("CurAnim", (int)E_PlAnimState.IDLE);
		m_anim.speed = 0.0f;
		m_transAtk = this.transform.Find("AtkPos").transform;
		UIMgr.GetInst ().GetHpText ().text = m_status.nCurHp.ToString();

		FindArmBone();
		FindHandBone();

		m_goExplosion = EffectMgr.GetInst ().GetEffect ((int)E_EFFECTID.DEATH);

		m_bodyMaterial.color = Color.white;
		m_arModeFunc [0]();
	}
		
	void FixedUpdate()
	{
		if (!StageMgr.GetInst ().GetPlay ())
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

	public override IEnumerator AttackUnit ()
	{
		if(m_goChip!=null)
		{
			yield break;
		}
		while(m_act==E_ACT.ATK)
		{
			if(StageMgr.GetInst().GetPlay()==false)
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
		m_status.nCurHp -= nDamage;
		m_fChargeTime = 0.0f;
		m_bCharged = false;
		StopCoroutine (AttackUnit());
		StopCoroutine (CamMgr.GetInst ().SubCamActive (this.transform));
		StopCoroutine (UseChip (0));
		CamMgr.GetInst ().GetSubCam ().SetActive (false);
		if(m_goChip!=null)
		{
			ObjectPool.GetInst ().PooledObject (m_goChip);
			m_goChip = null;
		}

		if (m_status.nCurHp <= 0) {
			UIMgr.GetInst ().GetHpText ().text = 0.ToString();
			m_act=E_ACT.DIE;
			m_anim.SetInteger ("CurAnim", (int)E_PlAnimState.FallDown);
			Invoke ("PooledThis", 2.0f);
			return;
		} 
		m_act = E_ACT.HIT;
		m_anim.SetInteger ("CurAnim", (int)E_PlAnimState.Hit);
		StartCoroutine (ChangeBodysAlpha());
		UIMgr.GetInst ().GetHpText ().text = m_status.nCurHp.ToString();
		Invoke ("ResetIdle", 0.35f);
		
	}
		
	public IEnumerator UseChip(int nIndex){
		if(m_act!=E_ACT.IDLE)
			yield break;
		m_fChargeTime = 0.0f;
		ChipData ChipTmp=ChipMgr.GetInst ().GetUseChipUse (nIndex);
		ChipMgr.GetInst ().ResetUseChip ();
		yield return null;
		m_anim.SetInteger ("CurAnim", ChipTmp.nAnimIndex);
		m_act=E_ACT.ATK;
		StartCoroutine (m_arChipFunc [(int)ChipTmp.eChipType] (ChipTmp));
		yield return null;
	}

	public void SetLock()
	{
		m_bLock = !m_bLock;
		if (m_bLock == true) 
		{
			StartCoroutine (LockUnit());
			return;
		}
		StopCoroutine (LockUnit());	
	}

	private IEnumerator LockUnit()
	{
		
		yield return null;
	}

	public IEnumerator UseShield()
	{
		StartCoroutine (CamMgr.GetInst ().SubCamActive (transform));
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
	private void ResetIdle()
	{
		m_act = E_ACT.IDLE;
		m_anim.SetInteger ("CurAnim", (int)E_PlAnimState.IDLE);
		StopCoroutine (ChangeBodysAlpha ());
	}

	private void PooledThis()
	{
		m_goExplosion=ObjectPool.GetInst().GetObject(m_goExplosion);
		m_goExplosion.transform.position = this.transform.position;
		m_CurPanel.SetPassable (true);
		StageMgr.GetInst ().StartCoroutine (ExplosionPool());
		ObjectPool.GetInst ().PooledObject (this.gameObject);

	}

	private IEnumerator ChangeBodysAlpha()
	{
		m_arModeFunc [3]();

		for(float f=0.0f;f<0.5f;f+=Time.deltaTime)
		{
			m_bodyMaterial.color = new Vector4(1.0f,1.0f,1.0f,0.3f);
			yield return null;
			m_bodyMaterial.color = new Vector4(1.0f,1.0f,1.0f,1.0f);
			yield return null;
		}

		m_arModeFunc [0]();

		yield return null;
	}


	private void InitializeDelegate()
	{
		m_arChipFunc = new delUseChip[(int)E_CHIPTYPE.MAX];

		m_arChipFunc [(int)E_CHIPTYPE.WEAPON] = UseWeapon;
		m_arChipFunc [(int)E_CHIPTYPE.ATKEFFECT] = UseAtkEffect;
		m_arChipFunc [(int)E_CHIPTYPE.SUMMON] = UseSummon;
		m_arChipFunc [(int)E_CHIPTYPE.RECOVERY] = UseRecovery;
		m_arChipFunc [(int)E_CHIPTYPE.ATTACH] = UseAttach;
		m_arChipFunc [(int)E_CHIPTYPE.AREA] = UseArea;
		m_arChipFunc [(int)E_CHIPTYPE.SUPPORT] = UseSupport;
		m_arChipFunc [(int)E_CHIPTYPE.NONE] = UseNone;
	}

	private IEnumerator UseWeapon(ChipData chipData)
	{
		StartCoroutine (CamMgr.GetInst ().SubCamActive (transform));
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

	private IEnumerator UseAtkEffect(ChipData chipData)
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
		ChipData chipTmp=new ChipData();
		chipTmp=ChipMgr.GetInst ().GetUseChip (0);

		if(chipTmp.eChipType!=E_CHIPTYPE.ATKEFFECT&&chipTmp.eChipType!=E_CHIPTYPE.WEAPON&&chipTmp.eChipType!=E_CHIPTYPE.SUMMON&&chipTmp.eChipType!=E_CHIPTYPE.ATTACH)
		{
			StartCoroutine(PoofFunc ());
			yield break;
		}

		chipTmp.nValue += chipData.nValue;
		ChipMgr.GetInst ().SetUseChip (0,chipTmp);
		chipTmp = ChipMgr.GetInst ().GetUseChip (0);
		Debug.Log (chipTmp.nValue);

		yield return null;

		ObjectPool.GetInst ().PooledObject (m_goChip);
		m_goChip = null;
		ResetIdle ();
		ChipMgr.GetInst ().ResetUseChip ();

		yield return null;
	}

	private IEnumerator UseArea(ChipData chipData)
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

		if(m_status.nCurHp>m_status.nMaxHp)
		{
			m_status.nCurHp = m_status.nMaxHp;
		}
		UIMgr.GetInst ().GetHpText ().text = m_status.nCurHp.ToString();

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

/*
m_goBuster.transform.parent = null;
		m_goBuster.transform.localScale = new Vector3 (0.0f, 0.0f, 0.0f);
		SkinnedMeshRenderer meshWeapon=goWeapon.transform.GetComponent<SkinnedMeshRenderer> ();
		meshWeapon.bones = m_BusterMesh.bones;

		m_BusterMesh.sharedMesh= meshWeapon.sharedMesh;
		m_BusterMesh.material = meshWeapon.material; 
*/