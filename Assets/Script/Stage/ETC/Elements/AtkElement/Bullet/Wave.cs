using UnityEngine;
using System.Collections;

public class Wave : AtkElement {
	private Wave()
	{
		m_bAllive = true;
		m_nValue = 0;
		m_fSpeed = 0.0f;
	}

	void Awake()
	{
		m_fSpeed = 3.0f;
	}
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

	}
	void OnEnable()
	{
		m_bAllive = true;
		StartCoroutine (Work ());
	}
	void OnDisable()
	{
		StopCoroutine (Work ());
		m_bAllive = false;
		m_Unit = null;
	}
	void OnTriggerEnter(Collider collider)
	{
		
		if(collider.GetComponent<UnitBase>()==null)
			return;
		UnitBase pBase=collider.GetComponent<UnitBase> ();

		if(pBase.GetType()==m_Unit.GetType())
			return;

		pBase.GetDamage (m_Unit.GetAttackBase().nDmg);
		PooledThis ();
	}



	protected override IEnumerator Work()
	{
		if(m_Unit==null)
		{
			yield return new WaitUntil (() => m_Unit != null);
		}
		transform.forward = m_Unit.transform.forward;
		transform.position = m_Unit.transform.position + transform.forward;
		while(m_bAllive)
		{
			transform.position += transform.forward*Time.deltaTime*m_fSpeed;
			if(Vector3.Distance(transform.position,m_Unit.transform.position)>=11.0f)
			{
				PooledThis();
				m_Unit = null;
				yield break;
			}
			yield return null;
		}
		yield return null;
	}
}
