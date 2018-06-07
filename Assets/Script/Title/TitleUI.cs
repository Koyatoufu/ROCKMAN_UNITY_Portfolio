using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class TitleUI : MonoBehaviour {

	private static TitleUI m_Inst = null;

	private GameObject m_objUI;
	private GameObject m_objQuitUI;
	private GameObject m_objPress;
	private GameObject m_objLogo;

	private Image m_imgBackground;
	private Image m_imgTitle;
	private Image m_imgLogo;


	private TitleUI()
	{
		m_objUI = null;
		m_imgBackground = null;
		m_imgTitle = null;
		m_imgLogo = null;
		m_objQuitUI = null;
		m_objPress = null;
		m_objLogo = null;
	}

	public static TitleUI GetInst()
	{
		return m_Inst;
	}

	void Awake()
	{
		m_Inst = this;
		m_objUI = this.gameObject;
		m_objQuitUI = transform.Find("Quit").gameObject;
		m_objPress = transform.Find("PressText").gameObject;
		m_objLogo = transform.Find("Logo").gameObject;
		m_imgBackground = transform.Find("Background").GetComponent<Image> ();
		m_imgTitle = transform.Find("Title").GetComponent<Image> ();
		m_imgLogo = m_objLogo.GetComponent<Image> ();
	}

	// Use this for initialization
	void Start () {
		m_imgBackground.CrossFadeColor (Color.black,0.0f,false,false);
		m_imgLogo.CrossFadeColor (Color.black,0.0f,false,false);
		m_imgTitle.CrossFadeAlpha(0.0f,0.0f,false);
		StartCoroutine (StartScean());
	}

	void FixedUpdate()
	{
		if(Input.GetKeyDown(KeyCode.Escape))
		{
			ActiveQuitUI ();
		}
	}
		
	public void ActiveQuitUI()
	{
		m_objQuitUI.SetActive (!m_objQuitUI.activeSelf);
		Time.timeScale = Time.timeScale == 0.0f ? 1.0f : 0.0f;
	}

	public GameObject GetObject()
	{
		return m_objUI;
	} 
	public Image GetImageBackground()
	{
		return m_imgBackground;
	}
	public Image GetImageTitle()
	{
		return m_imgTitle;
	}
	IEnumerator StartScean()
	{
		m_imgLogo.CrossFadeColor (Color.white,3.0f,false,false);
		yield return new WaitForSeconds (3.0f);
		m_imgLogo.CrossFadeColor (Color.black,3.0f,false,false);
		yield return new WaitForSeconds (3.0f);
		m_objLogo.SetActive (false);
		m_imgBackground.CrossFadeColor (Color.white,3.0f,false,false);
		yield return new WaitForSeconds (3.0f);
		StartCoroutine (TitleAppear());
		yield return new WaitForSeconds (0.5f);
	}
	IEnumerator TitleAppear()
	{
		m_imgTitle.gameObject.SetActive (true);
		m_imgTitle.CrossFadeAlpha(1.0f,5.0f,false);
		yield return new WaitForSeconds (3.0f);
		TitleMgr.GetInst ().BgmOn ();
		yield return new WaitForSeconds (2.0f);
		m_objPress.SetActive (true);
		TitleMgr.GetInst ().SetProgress ();
		yield return null;
	}
}
