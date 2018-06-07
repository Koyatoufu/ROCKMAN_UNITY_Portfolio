using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChipMainMgr : MonoBehaviour {

	private static ChipMainMgr m_Inst=null;

	private GameObject m_objMainUI;

	private List<ChipData> m_Chips;

	private ChipMainMgr()
	{
		m_objMainUI = null;
		m_Chips = new List<ChipData>();
	}
	public static void InitInst()
	{
		if(m_Inst==null)
		{
			m_Inst = new ChipMainMgr ();
		}
	}

	public static ChipMainMgr GetInst()
	{
		return m_Inst;
	}

	void Awake()
	{
		m_Inst = this;
		m_objMainUI = transform.Find("").gameObject;
		DBMgr.InitInst ();
		DBMgr.GetInst ().OpenConnection ();


	}

	// Use this for initialization
	void Start () {
			
		string szPath = Application.dataPath + "/xmlFile/ChipFolder/BaseFolder.xml";

		ChipXMlFileMgr.LoadXml (szPath,m_Chips);

		for(int i=0;i<m_Chips.Count;i++)
		{
			
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public GameObject GetMainUI()
	{
		return m_objMainUI;
	}
}
