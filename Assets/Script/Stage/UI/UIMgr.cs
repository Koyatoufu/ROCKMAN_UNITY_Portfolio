using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIMgr :MonoBehaviour{

	private GameObject m_goMainUI;
	private GameObject m_goCustom;

	private Animator m_animGauge;
	private Image m_ImgGauge;

	private Text m_textPlayerHP;
	private Text m_textCardInfo;

	private static UIMgr m_Inst = null;

	private bool m_bPause=false;

	private bool m_bGamed=false;
	private bool m_bCustomed=false;

	private GameObject[] m_arGoChips;
	private Image m_imgChip;
	private Image[] m_arIconImgs;
	private Text[] m_arTextCodes;
	private Text m_textSelect;

	private Sprite m_spriteMask;

	private Image[] m_arSelectIcons;
	private Image[] m_arUseIcons;

	private GameObject m_goMsg;
	private Animator m_animMsg;
	private AudioSource m_audioMsg;

	private GameObject m_goStateBack;
	private Animator m_animState;

	private GameObject m_goPausedButton;
	private GameObject m_goPausedMenu;

	private UIMgr()
	{
		m_goMainUI = null;
		m_goCustom = null;

		m_ImgGauge = null;

		m_arGoChips = new GameObject[10];
		m_imgChip = null;

		m_textCardInfo = null;
		m_arIconImgs = new Image[10];
		m_arTextCodes = new Text[10];

		m_goMsg = null;
		m_animMsg = null;
		m_audioMsg = null;

		m_arSelectIcons = new Image[5];
		m_arUseIcons = new Image[5];
		m_animState = null;

		m_goPausedButton = null;
		m_goPausedMenu = null;
	}

	public static UIMgr GetInst()
	{
		return m_Inst;
	}

	void Awake()
	{
		m_Inst = this;

		GameObject gaugeObj = transform.Find("BattleMain").Find("CustomBar").Find("Btn_Custom").gameObject;
		m_animGauge = gaugeObj.GetComponent<Animator> ();

		m_ImgGauge = gaugeObj.GetComponent<Image> ();

		m_goMainUI = transform.Find("BattleMain").gameObject;
		m_goCustom = transform.Find("CustomScean").gameObject;
		m_textPlayerHP=transform.Find("HP").Find("HPText").GetComponent<Text>();
		m_textCardInfo = m_goMainUI.transform.Find("BattleCardSlot").Find("CardText").GetComponent<Text>();

		m_goMsg = transform.Find("Message").gameObject;
		m_animMsg = m_goMsg.GetComponent<Animator> ();
		m_audioMsg = m_goMsg.GetComponent<AudioSource> ();

		m_goStateBack = transform.Find("StateBack").gameObject;
		m_animState = m_goStateBack.transform.GetComponent<Animator> ();

		m_goPausedMenu = transform.Find("PausedMenu").gameObject;
		m_goPausedButton = transform.Find("Btn_Pause").gameObject;

		InitaializeChipsImg ();
	}

	void Start()
	{
		m_goMainUI.SetActive (false);
		m_goCustom.SetActive (false);
		m_bCustomed = false;
	}

	void FixedUpdate()
	{
		if(m_goMainUI.activeSelf)
		{
			m_ImgGauge.fillAmount += Time.fixedDeltaTime*0.1f;
			if(m_ImgGauge.fillAmount>=1)
			{
				m_animGauge.SetBool ("Full",true);
			}
		}
	}

	public void ClearSelectedIcons()
	{
		for(int i=0;i<5;i++)
		{
			m_arSelectIcons [i].sprite = m_spriteMask;
		}

		m_imgChip.sprite = m_spriteMask;
		m_textSelect.text = "";
	}
	public void ClearUseChips()
	{
		for(int i=0;i<5;i++)
		{
			m_arUseIcons [i].sprite = m_spriteMask;
		}
	}
	public void ClearChip()
	{
		for(int i=0;i<10;i++)
		{
			m_arIconImgs [i].sprite = m_spriteMask;
			m_arTextCodes[i].text = " ";
		}
	}

	public void SetActiveMain()
	{
		bool bActive = m_goMainUI.activeSelf;

		m_goMainUI.SetActive (!bActive);

		m_bGamed = !bActive;
	}
	public void SetActiveMain(bool bActive)
	{
		m_goMainUI.SetActive (bActive);
		m_bGamed = bActive;
	}
	public void SetActiveCustom()
	{
		bool bActive = m_goCustom.activeSelf;

		m_goCustom.SetActive (!bActive);

		m_bCustomed = !bActive;
	}

	public void SetCustomConfirm()
	{
		StageMgr.GetInst ().PlusCurTurn ();

		m_goCustom.SetActive (false);

		m_bCustomed = false;
	}

	public void SetImageChips(Sprite sprite)
	{
		m_imgChip.sprite=sprite;
	}
	public void SetImageChipsMask()
	{
		m_imgChip.sprite = m_spriteMask;
	}

	public void SetActivePause()
	{
		if(m_bPause==true)
		{
			m_bPause = false;
			if(m_bCustomed==true)
			{
				m_goCustom.SetActive (true);
			}
			else if(m_bGamed==true)
			{
				m_goMainUI.SetActive (true);
			}
			m_goPausedMenu.SetActive (false);
			m_goPausedButton.SetActive (true);
		}
		else
		{
			m_bPause = true;
			m_goCustom.SetActive (false);
			m_goMainUI.SetActive (false);
			m_goPausedMenu.SetActive (true);
			m_goPausedButton.SetActive (false);
		}
	}

	public Animator GetGaugeAnim()
	{
		return m_animGauge;
	}

	public Image GetGaugeImage()
	{
		return m_ImgGauge;
	}

	public Image[] GetImageIcon()
	{
		return m_arIconImgs;
	}
	public Text[] GetCodeText()
	{
		return m_arTextCodes;
	}
	public Text GetCardInfoText()
	{
		return m_textCardInfo;
	}
	public Text GetSelectChipText()
	{
		return m_textSelect;
	}

	public Text GetHpText()
	{
		return m_textPlayerHP;
	}

	public bool GetCustomed()
	{
		return m_bCustomed;
	}
	public GameObject GetMsgUI()
	{
		return m_goMsg;
	}
	public Animator GetAnimMsgUI()
	{
		return m_animMsg;
	}
	public AudioSource GetAudioMsgUI()
	{
		return m_audioMsg;
	}
	public Animator GetAnimStateUI()
	{
		m_goStateBack.SetActive (true);
		return m_animState;
	}

	public Image GetSelectedIcons(int nIndex)
	{
		return m_arSelectIcons [nIndex];
	}
	public Image GetUseIcons(int nIndex)
	{
		return m_arUseIcons [nIndex];
	}

	private void InitaializeChipsImg()
	{
		Transform goChips = m_goCustom.transform.Find("CustomMain").Find("ChipsInfo");
		m_imgChip =m_goCustom.transform.Find("CustomMain").Find("ChipImage").GetComponent<Image>();
		m_textSelect = m_goCustom.transform.Find("CustomMain").Find ("ChipText").GetComponent<Text>();
		m_textSelect.text = "";

		Texture2D tex2Dmask = Resources.Load <Texture2D>("mask");
		m_spriteMask = Sprite.Create (tex2Dmask, new Rect (0, 0, tex2Dmask.width, tex2Dmask.height), new Vector2 (0, 0));

		for(int i=0;i<10;i++)
		{
			string szTmp = "Chip_"+i;
			m_arGoChips [i] = goChips.Find (szTmp).gameObject;
			m_arIconImgs [i] = m_arGoChips [i].transform.Find ("Icon").GetComponent<Image> ();
			m_arIconImgs [i].sprite = m_spriteMask;
			m_arTextCodes [i] = m_arGoChips [i].transform.Find ("Code").GetComponent<Text> ();
		}

		Transform goSelect = m_goCustom.transform.Find ("CustomMain").Find("SelectChips");

		for(int i=0;i<5;i++)
		{
			string szTmp = "Icon_" + i;
			m_arSelectIcons [i] = goSelect.Find (szTmp).GetComponent<Image> ();
			m_arSelectIcons [i].sprite = m_spriteMask;
		}

		Transform goChipSlot = m_goMainUI.transform.Find ("BattleCardSlot");

		for(int i=0;i<5;i++)
		{
			string szTmp = "Chip_" + i;
			m_arUseIcons [i] = goChipSlot.Find (szTmp).GetComponent<Image> ();
			m_arUseIcons [i].sprite = m_spriteMask;
		}
	}


}
