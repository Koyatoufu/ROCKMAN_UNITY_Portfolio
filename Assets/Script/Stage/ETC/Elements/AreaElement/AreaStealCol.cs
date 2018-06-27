using UnityEngine;
using System.Collections;

public class AreaStealCol : AreaElement {

	private GameObject m_goEffect;
	private GameObject m_goCol;

	void Awake()
	{
		m_backParent = transform.parent;
		m_nValue = 10;

		m_goEffect = transform.Find ("effect").gameObject;
		m_goCol = transform.Find ("col").gameObject;
	}

	void OnEnable()
	{
		m_bAllive = true;
		m_goEffect.SetActive (true);
		m_goCol.SetActive (false);
		transform.parent = null;
		StartCoroutine (ExecuteCoroutine ());
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
				StopCoroutine (ExecuteCoroutine ());
				Invoke ("PooledThis",0.5f);
				return;
			}
		}

		Panel pCol = collider.transform.GetComponent<Panel> ();

        if(pCol!=null)
        {
            int nX = pCol.GetPoint().nX;
            int nZ = pCol.GetPoint().nZ;

            if (PhotonNetwork.room != null)
            {
                ChangePanelColor(pCol,nX,nZ);
            }   
            else
                ChangePanelColor(pCol);
        }
        
        StopCoroutine(ExecuteCoroutine());
        Invoke ("PooledThis",0.25f);
	}

    public void ChangePanelColor(Panel pCol)
    {
        if (pCol.IsRed == m_panel.IsRed)
        {
            PooledThis();
            return;
        }
        m_goEffect.SetActive(false);
        m_goCol.SetActive(true);

        pCol.ReversePanel();
    }

    public void ChangePanelColor(Panel pCol,int nX,int nZ)
    {
        if (pCol.IsRed == m_panel.IsRed)
        {
            PooledThis();
            return;
        }

        m_goEffect.SetActive(false);
        m_goCol.SetActive(true);

        if (m_Unit.photonView.isMine)
            m_Unit.photonView.RPC("ChangePanelColor", PhotonTargets.AllBufferedViaServer, nX, nZ);
    }

	protected override IEnumerator ExecuteCoroutine ()
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

	public override void PooledThis ()
	{
		transform.parent = m_backParent;
		transform.gameObject.SetActive (false);
	}
}
