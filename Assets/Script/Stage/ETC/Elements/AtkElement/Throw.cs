using UnityEngine;
using System.Collections;

public class Throw : AtkElement {

	private float m_fStartTime;

	private GameObject m_goExplosion;

	private Throw()
	{
		
	}

	void Awake()
	{
		m_fSpeed = 1.0f;
		m_fStartTime = 0.0f;
	}

	// Use this for initialization
	void Start () {
		
	}

	void OnEnable()
	{
		m_bAllive = true;
		StartCoroutine (Work());
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
		UnitBase unitTmp = collider.GetComponent<UnitBase> ();
		if(unitTmp!=null)
		{
			if (unitTmp.GetType () == m_Unit.GetType ())
				return;
			unitTmp.GetDamage (m_nValue);
		}
		ElementBase eleTmp = collider.GetComponent<ElementBase> ();
		if(eleTmp!=null)
		{
			if(eleTmp.GetUnit().GetType()==m_Unit.GetType())
				return;
		}

		m_goExplosion=ObjectPool.GetInst ().GetObject (EffectMgr.GetInst().GetEffect((int)E_EFFECTID.EXPLOSION));

		m_goExplosion.transform.position = transform.position;

		PooledThis ();
	}

	protected override IEnumerator Work ()
	{
		yield return new WaitUntil(()=>m_Unit!=null);
		Vector3 vTarget = MapMgr.GetInst ().GetMapPanel (m_Unit.GetCurPanel ().GetPoint ().nX+3, m_Unit.GetCurPanel ().GetPoint ().nZ).transform.position;
		while(m_bAllive)
		{
			Vector3 vCenter = (transform.position + vTarget) * 0.5f;
			vCenter.y -= 1.5f;
			Vector3 vRelCenter = transform.position - vCenter;
			Vector3 vTargetRelCenter = vTarget - vCenter;
			float fFraccomplete = (Time.time - m_fStartTime) *0.125f;
			transform.position = Vector3.Slerp (vRelCenter,vTargetRelCenter,fFraccomplete);
			transform.position += vCenter;
			transform.Rotate (new Vector3 (0.0f, 1.0f, 0.0f),Time.deltaTime*5.0f);
			yield return null;
		}
	}

	protected override void PooledThis ()
	{
		StageMgr.GetInst().StartCoroutine (PooledExplosion());	
		base.PooledThis ();
	}

	private IEnumerator PooledExplosion()
	{
		yield return new WaitForSeconds (0.5f);
		GameObject objTmp = m_goExplosion;
		m_goExplosion = null;
		ObjectPool.GetInst ().PooledObject (objTmp);
		yield return null;
	}
}
/*
 * //포물선
	GameObject player;
	Vector3 startPos; 
	Vector3 destPos;
	float timer; float vx; float vy; float vz; 
	PlayerHealth playerHealth; // Use this for initialization
	void Start () 
	{
	player = GameObject.FindGameObjectWithTag ("Player"); playerHealth = player.GetComponent ();
	startPos = transform.position; destPos = player.transform.position; vx = (destPos.x - startPos.x) / 2f;
	vz = (destPos.z - startPos.z) / 2f; vy = (destPos.y - startPos.y + 2 * 9.8f) / 2f; 
	}
	// Update is called once per frame 
	void Update () 
	{ 
	timer += Time.deltaTime;
	float sx = startPos.x + vx * timer;
	float sy = startPos.y + vy * timer - 0.5f * 9.8f * timer * timer;
	float sz = startPos.z + vz * timer; 
	transform.position = new Vector3(sx, sy, sz); 
	transform.Rotate (1.2f, 0, 0); 
	}
*/