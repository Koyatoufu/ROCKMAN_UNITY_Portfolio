using UnityEngine;
using System.Collections;

public class Slash : AtkElement {

	protected ParticleSystem m_particle;

	protected Slash()
	{
		m_particle = null;
	}

	void Awake()
	{
		m_particle = transform.GetComponent<ParticleSystem> ();
		m_backParent = transform.parent;
	}

	// Use this for initialization
	void Start () {
	}

	void OnEnable()
	{
		m_backParent = transform.parent;
		StartCoroutine (Work ());
		m_particle.Stop ();
	}
	void OnDisable()
	{
		StopCoroutine (Work ());
		m_Unit = null;
	}
	void OnTriggerEnter(Collider collider)
	{

		if (m_Unit == null)
			return;

		UnitBase pBase=collider.GetComponent<UnitBase> ();

		if(pBase==null)
			return;

		if(pBase.GetType()==m_Unit.GetType())
			return;
		
		pBase.GetDamage (m_nValue);

	}

	protected override IEnumerator Work ()
	{
		yield return new WaitUntil (()=>m_Unit!=null);
		transform.parent = null;
		transform.position = m_Unit.GetTransformAtk ().position;
		transform.rotation = Quaternion.identity;
		transform.forward = m_Unit.transform.forward;
        ParticleSystem.MainModule mainTemp = m_particle.main;
        mainTemp.startRotation = 0.0f;
        mainTemp.startRotation = 0.0f;
		if(transform.forward.x>0)
		{
			mainTemp.startRotation = 3.141592f;
		}

		m_particle.Play ();
		yield return new WaitForSeconds (0.5f);

		PooledThis ();

		yield return null;
	}

	protected override void PooledThis ()
	{
		if(m_backParent!=null)
		{
			transform.parent = m_backParent;
			if(transform.parent.parent!=null)
			{
				ObjectPool.GetInst ().PooledObject (transform.parent.parent.gameObject);
				return;
			}
		}
		base.PooledThis ();
	}
}
