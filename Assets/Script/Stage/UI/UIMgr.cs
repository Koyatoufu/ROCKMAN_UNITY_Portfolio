using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIMgr : Photon.MonoBehaviour
{

    #region SingleTone

    private static UIMgr m_Inst = null;
    public static UIMgr Inst { get { return m_Inst; } }

    #endregion

    #region Instance

    private Animator m_animGauge = null;
	private Image m_ImgGauge = null;

    #region Serialize
    [SerializeField]
	protected Text m_textPlayerHP = null;
    [SerializeField]
	protected Text m_textCardInfo = null;
    [SerializeField]
    protected GameObject m_guageGo = null;
    [SerializeField]
    protected GameObject m_mainUIGo = null;
    [SerializeField]
    protected GameObject m_customGo = null;
    [SerializeField]
    private GameObject m_pausedButtonGo = null;
    [SerializeField]
    private GameObject m_pausedMenuGo = null;
    [SerializeField]
    private GameObject m_msgGo = null;
    [SerializeField]
    private GameObject m_stateBackGo = null;

    [Header("CustomChip")]
    [SerializeField]
    private Image m_chipImg = null;
    [SerializeField]
    private Text m_textSelect = null;
    [SerializeField]
    private Transform m_chipInfo = null;
    [SerializeField]
    private Transform m_selectedChip = null;
    [SerializeField]
    private Transform m_battleSlot = null;
    #endregion

    protected bool m_bPause=false;

	protected bool m_bGamed=false;
	protected bool m_bCustomed=false;

	private GameObject[] m_arGoChips = new GameObject[10];
	
	private Image[] m_arIconImgs = new Image[10];
	private Text[] m_arTextCodes = new Text[10];

	private Sprite m_spriteMask = null;

	private Image[] m_arSelectIcons = new Image[5];
	private Image[] m_arUseIcons = new Image[5];

	private Animator m_animMsg = null;
	private AudioSource m_audioMsg = null;

	private Animator m_animState = null;

    #endregion

    #region Initialize

    protected virtual void Awake()
	{
		m_Inst = this;

		m_animGauge = m_guageGo.GetComponent<Animator> ();

		m_ImgGauge = m_guageGo.GetComponent<Image> ();

		m_animMsg = m_msgGo.GetComponent<Animator> ();
		m_audioMsg = m_msgGo.GetComponent<AudioSource> ();

		m_animState = m_stateBackGo.transform.GetComponent<Animator> ();

		InitaializeChipsImg ();
	}

	protected virtual void Start()
	{
		m_mainUIGo.SetActive (false);
		m_customGo.SetActive (false);
		m_bCustomed = false;
	}

    protected void InitaializeChipsImg()
    {
        m_textSelect.text = "";

        Texture2D tex2Dmask = Resources.Load<Texture2D>("mask");
        m_spriteMask = Sprite.Create(tex2Dmask, new Rect(0, 0, tex2Dmask.width, tex2Dmask.height), new Vector2(0, 0));

        for (int i = 0; i < 10; i++)
        {
            string szTmp = "Chip_" + i;
            m_arGoChips[i] = m_chipInfo.Find(szTmp).gameObject;
            m_arIconImgs[i] = m_arGoChips[i].transform.Find("Icon").GetComponent<Image>();
            m_arIconImgs[i].sprite = m_spriteMask;
            m_arTextCodes[i] = m_arGoChips[i].transform.Find("Code").GetComponent<Text>();
        }

        for (int i = 0; i < 5; i++)
        {
            string szTmp = "Icon_" + i;
            m_arSelectIcons[i] = m_selectedChip.Find(szTmp).GetComponent<Image>();
            m_arSelectIcons[i].sprite = m_spriteMask;
        }

        for (int i = 0; i < 5; i++)
        {
            string szTmp = "Chip_" + i;
            m_arUseIcons[i] = m_battleSlot.Find(szTmp).GetComponent<Image>();
            m_arUseIcons[i].sprite = m_spriteMask;
        }
    }

    #endregion

    void Update()
    {
        if (!StageMgr.Inst.IsPlay)
            return;

        if (m_mainUIGo.activeSelf)
        {
            m_ImgGauge.fillAmount += Time.deltaTime * 0.1f;
            if (m_ImgGauge.fillAmount >= 1)
            {
                m_animGauge.SetBool("Full", true);
            }
        }
    }

    public void ClearSelectedIcons()
	{
		for(int i=0;i<5;i++)
		{
			m_arSelectIcons [i].sprite = m_spriteMask;
		}

		m_chipImg.sprite = m_spriteMask;
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

    #region Setter

    public void SetActiveMain()
	{
		bool bActive = m_mainUIGo.activeSelf;

		m_mainUIGo.SetActive (!bActive);

		m_bGamed = !bActive;
	}
	public void SetActiveMain(bool bActive)
	{
		m_mainUIGo.SetActive (bActive);
		m_bGamed = bActive;
	}
	public void SetActiveCustom()
	{
		bool bActive = m_customGo.activeSelf;

		m_customGo.SetActive (!bActive);

		m_bCustomed = !bActive;
	}

	public void SetCustomConfirm()
	{
		StageMgr.Inst.PlusCurTurn ();

		m_customGo.SetActive (false);

		m_bCustomed = false;
	}

	public void SetImageChips(Sprite sprite)
	{
		m_chipImg.sprite=sprite;
	}
	public void SetImageChipsMask()
	{
		m_chipImg.sprite = m_spriteMask;
	}

	public void SetActivePause()
	{
		if(m_bPause==true)
		{
			m_bPause = false;
			if(m_bCustomed==true)
			{
				m_customGo.SetActive (true);
			}
			else if(m_bGamed==true)
			{
				m_mainUIGo.SetActive (true);
			}
			m_pausedMenuGo.SetActive (false);
			m_pausedButtonGo.SetActive (true);
		}
		else
		{
			m_bPause = true;
			m_customGo.SetActive (false);
			m_mainUIGo.SetActive (false);
			m_pausedMenuGo.SetActive (true);
			m_pausedButtonGo.SetActive (false);
		}
	}
    #endregion
    #region Getter

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
		return m_msgGo;
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
		m_stateBackGo.SetActive (true);
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

    #endregion

}
