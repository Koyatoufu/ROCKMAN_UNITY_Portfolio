using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Enemy : UnitBase {

	//private Material [] m_arBodyMaterials;

	public Enemy()
	{
		m_status.szName = "Mattol";
		m_act = E_ACT.IDLE;
		m_AI = new AI ();
	}

	protected override void Awake()
	{
        base.Awake();

		m_anim = transform.GetComponent<Animator> ();

		m_status.nMaxHp = 40;
		m_status.nCurHp = m_status.nMaxHp;
		m_status.fMoveSpeed = 5.0f;

		m_baseAtk.nDmg = 10;
		m_baseAtk.eType = E_TYPE.NONE;
		m_baseAtk.fSpeed = 3.0f;
		m_baseAtk.objType = ElementMgr.GetInst().GetElement((int)E_ATKELEMENT.WAVE);

		m_AI.SetUnit (this);

	}

	// Use this for initialization
	void Start () {
        IsRed = true;

        m_anim.speed = 0.0f;

		m_arModeFunc [3]();

		StartCoroutine (FadeIn());
		m_textHp.text = m_status.nCurHp.ToString ();

		m_goExplosion = EffectMgr.GetInst ().GetEffect ((int)E_EFFECTID.EXPLOSION);
	}

	public IEnumerator FadeIn()
	{
		for(float f=0.0f;f<=1.0f;f+=0.025f)
		{
			/*for(int i=0;i<m_arBodyMaterials.Length;i++)
			{
				m_arBodyMaterials [i].color = new Vector4(1.0f,1.0f,1.0f,f);
			}*/

			m_bodyMaterial.color = new Vector4 (1.0f, 1.0f, 1.0f, f);

			yield return null;
		}

		m_arModeFunc [0]();

		yield return null;

	}

	public override void GetDamage(int nDamage)
	{
		m_status.nCurHp -= nDamage;
		if (m_status.nCurHp <= 0) {
			m_CurPanel.Passable = true;
			m_act=E_ACT.DIE;
			m_goExplosion=ObjectPool.GetInst().GetObject(m_goExplosion);
			m_goExplosion.transform.position = transform.position+new Vector3(0.0f,0.5f,0.0f);
			StageMgr.Inst.StartCoroutine (ExplosionPool ());
			ObjectPool.GetInst ().PooledObject (this.gameObject);
			UnitMgr.Inst.GetEnemyList ().Remove (this);
		} else {
			m_textHp.text = m_status.nCurHp.ToString ();
		}
	}

	//Color color=new Color(1.0f,0,1.0f);
	//gameObject.transform.FindChild ("Helmet").GetComponent<Renderer>().material.color=color;
}

/*
	if(m_act==E_ACT.MOVE){
			Panel panelNext = m_listMovePanel [0];
			float fDistance = Vector3.Distance (transform.position,panelNext.transform.position);
			if (fDistance >= 0.5f) {
				transform.position += (panelNext.transform.position-transform.position).normalized * Time.deltaTime * 5.0f;
			} 
			else {
				transform.position = panelNext.transform.position;
				m_listMovePanel.RemoveAt (0);
				if(m_listMovePanel.Count==0)
				{
					m_act = E_ACT.IDLE;
					m_CurPanel = panelNext;
				}
			}
		}
	*/
//Color color=new Color(1.0f,0,1.0f);
//gameObject.transform.FindChild ("Helmet").GetComponent<Renderer>().material.color=color;
