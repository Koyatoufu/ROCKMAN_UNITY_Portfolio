using UnityEngine;
using System.Collections;

public class TitleMgr : MonoBehaviour {

	private static TitleMgr m_Inst=null;

	private bool m_bTitle = false;
	private bool m_bProgress = false;
    private bool m_bLoad = false;

	private AudioSource m_audioBgm = null;

	private AudioSource m_audioStart;

	public static TitleMgr GetInst()
	{
		return m_Inst;
	}

	void Awake()
	{
		m_Inst = this;
		m_audioBgm = transform.GetComponent<AudioSource> ();
		m_audioStart = transform.Find ("TitleUI").GetComponent<AudioSource>();
	}

    /*
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
    */

    public void StageStart()
    {
        if (!m_bProgress)
            return;
        if (m_bLoad)
            return;
        StartCoroutine(StageCoroutine());
        m_bLoad = true;
    }

    public void NetworkStart()
    {
        if (!m_bProgress)
            return;
        if (m_bLoad)
            return;
        StartCoroutine(NetWorkScnenCoroutine());
        m_bLoad = true;
    }

    private IEnumerator StageCoroutine()
	{
		yield return null;
		m_audioStart.Play ();
		yield return new WaitForSeconds (0.8f);
		UnityEngine.SceneManagement.SceneManager.LoadScene("TestStage");
		yield return null;
	}

    private IEnumerator NetWorkScnenCoroutine()
    {
        yield return null;
        m_audioStart.Play();
        yield return new WaitForSeconds(0.8f);
        UnityEngine.SceneManagement.SceneManager.LoadScene("Lobby");
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
