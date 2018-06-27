using UnityEngine;
using System.Collections;

public class Slash : AtkElement {

	protected ParticleSystem m_particle = null;

	void Awake()
	{
		m_particle = transform.GetComponent<ParticleSystem> ();
		m_backParent = transform.parent;
	}

	void OnEnable()
	{
		m_backParent = transform.parent;
		StartCoroutine (ExecuteCoroutine ());
		m_particle.Stop ();
	}
	void OnDisable()
	{
		StopCoroutine (ExecuteCoroutine ());
		m_Unit = null;
	}

	void OnTriggerEnter(Collider collider)
	{
        if (m_Unit == null)
			return;

		UnitBase pBase=collider.GetComponent<UnitBase> ();

		if(pBase==null)
			return;

		if(pBase==m_Unit)
			return;

        DamageFunc(pBase);

	}

	protected override IEnumerator ExecuteCoroutine ()
	{
		yield return new WaitUntil (()=>m_Unit!=null);
		transform.parent = null;
		transform.position = m_Unit.GetTransformAtk ().position;
		transform.rotation = Quaternion.identity;
		transform.forward = m_Unit.transform.forward;
        ParticleSystem.MainModule main = m_particle.main;
        main.startRotation = 0.0f;
		if(transform.forward.x>0)
		{
            main.startRotation = 3.141592f;
		}

		m_particle.Play ();
		yield return new WaitForSeconds (0.5f);

		PooledThis ();

		yield return null;
	}

	public override void PooledThis ()
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

        Debug.Log(m_backParent);
		base.PooledThis ();
	}
}
