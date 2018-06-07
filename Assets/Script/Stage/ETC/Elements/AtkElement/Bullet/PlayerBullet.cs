using UnityEngine;
using System.Collections;

public class PlayerBullet : AtkElement {

	private Transform m_Hit;
	private Transform m_quad;

	void Awake()
	{
		m_fSpeed = 10.0f;
		m_Hit = this.transform.Find("Hit");
		m_quad = this.transform.Find("Quad");
		m_rigidBody = this.transform.GetComponent<Rigidbody> ();
	}

	// Use this for initialization
	void Start () {
	
	}

	void OnEnable()
	{
		m_Hit.gameObject.SetActive (false);
		m_quad.gameObject.SetActive (true);
		StartCoroutine (Work ());
		m_bAllive = true;
	}
	void OnDisable()
	{
		
		m_bAllive = false;
	}

	void OnTriggerEnter(Collider collider)
	{
		
		UnitBase pBase=collider.GetComponent<UnitBase> ();
		if (pBase == null)
			return;
		if(pBase.GetType()==m_Unit.GetType())
			return;
		pBase.GetDamage (m_nValue);
		m_Hit.gameObject.SetActive (true);
		m_quad.gameObject.SetActive (false);
		m_rigidBody.velocity = new Vector3(0.0f,0.0f,0.0f);
		Invoke ("PooledThis",0.1f);
	}

	protected override IEnumerator Work ()
	{
		yield return new WaitUntil (()=>m_Unit!=null);
		m_rigidBody.velocity=m_Unit.transform.forward*m_fSpeed*m_Unit.GetAttackBase().fSpeed;
		transform.rotation = m_Unit.transform.rotation;
		yield return null;
		while(m_bAllive)
		{
			if(Vector3.Distance(transform.position,m_Unit.transform.position)>=11.0f)
			{
				m_rigidBody.velocity = new Vector3(0.0f,0.0f,0.0f);
				PooledThis ();
				yield break;
			}
			yield return null;
		}
		yield return null;
	}
}
