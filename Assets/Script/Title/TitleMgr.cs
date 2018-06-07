using UnityEngine;
using System.Collections;

public class TitleMgr : MonoBehaviour {

	private static TitleMgr m_Inst=null;

	private bool m_bTitle;
	private bool m_bProgress;

	private AudioSource m_audioBgm;

	private AudioSource m_audioStart;

	private TitleMgr()
	{
		m_bTitle = false;
		m_bProgress = false;
		m_audioBgm = null;
	}

	public static TitleMgr GetInst()
	{
		return m_Inst;
	}

	void Awake()
	{
		m_Inst = this;
		m_audioBgm = transform.GetComponent<AudioSource> ();
		m_audioStart = transform.Find("TitleUI").GetComponent<AudioSource>();
	}

	// Use this for initialization
	void Start () {
		
	}

	void FixedUpdate()
	{
		if(m_bProgress)
		{
			if(Input.touchCount>0||Input.GetMouseButton(0))
			{
				StartCoroutine (StartScene ());
			}
		}
	}

	private IEnumerator StartScene()
	{
		yield return null;
		m_audioStart.Play ();
		yield return new WaitForSeconds (0.8f);
		UnityEngine.SceneManagement.SceneManager.LoadScene("TestStage");
		yield return null;
	}

	public void QuitGame()
	{
		Debug.Log ("Quit");
		Application.Quit ();
	}

	public bool GetBTitle()
	{
		return m_bTitle;
	}
		
	public void SetProgress()
	{
		m_bProgress = true;
	}
	public void BgmOn()
	{
		m_audioBgm.enabled = true;
	}
}
