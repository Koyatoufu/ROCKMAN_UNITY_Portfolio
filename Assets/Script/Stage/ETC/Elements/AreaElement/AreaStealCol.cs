using UnityEngine;
using System.Collections;

public class AreaStealCol : AreaElement {

	private GameObject m_goEffect;
	private GameObject m_goCol;

	void Awake()
	{
		m_backParent = transform.parent;
		m_nValue = 10;

		m_goEffect = transform.Find("effect").gameObject;
		m_goCol = transform.Find("col").gameObject;
	}

	void OnEnable()
	{
		m_bAllive = true;
		m_goEffect.SetActive (true);
		m_goCol.SetActive (false);
		transform.parent = null;
		StartCoroutine (Work ());
	}
	void OnDisable()
	{
		m_Unit = null;
		m_bAllive = false;
	}

	void OnTriggerEnter(Collider collider)
	{
		UnitBase pBase=collider.GetComponent<UnitBase> ();
		if (pBase != null) 
		{
			if (pBase.GetType () != m_Unit.GetType ()) 
			{
				pBase.GetDamage (m_nValue);
				m_goEffect.SetActive (false);
				m_goEffect.SetActive (true);
				StopCoroutine (Work ());
				Invoke ("PooledThis",0.5f);
				return;
			}
		}

		Panel pCol = collider.transform.GetComponent<Panel> ();
		if (pCol == null)
			return;
		if(pCol.GetTagName().CompareTo(m_panel.GetTagName())==0)
		{
			Debug.Log (pCol.GetTagName());
			PooledThis ();
			return;
		}
		m_goEffect.SetActive (false);
		m_goCol.SetActive (true);

		pCol.SetTexture (m_panel.GetOriTexture());
		pCol.SetTagName (m_panel.GetTagName());


		StopCoroutine (Work ());
		Invoke ("PooledThis",0.25f);
	}

	protected override IEnumerator Work ()
	{
		
		yield return new WaitUntil (()=>m_Unit!=null);

		m_panel = m_Unit.GetCurPanel ();

		while(m_bAllive)
		{
			transform.position += -(transform.up) * Time.deltaTime*2.5f;

			if(transform.position.y<0)
			{
				PooledThis ();
				yield break;
			}
			yield return null;
		}

		yield return null;
	}

	protected override void PooledThis ()
	{
		transform.parent = m_backParent;
		transform.gameObject.SetActive (false);
	}
}
