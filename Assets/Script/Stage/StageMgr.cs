using UnityEngine;
using System.Collections;

public class StageMgr : MonoBehaviour {

	private static StageMgr m_pInst=null;
	private bool m_bplay;
	private AudioSource m_audioBgm;
	private AudioClip m_clipDeleteEnemy;
	private AudioClip m_clipGameover;

	private int m_nCurTrun=0;

	private StageMgr()
	{
		m_pInst = this;
		m_bplay = false;
		m_audioBgm = null;
		m_clipDeleteEnemy = null;
	}

	public static StageMgr GetInst(){
		return m_pInst;
	}

	void Awake()
	{
		DBMgr.InitInst ();
		MapMgr.InitInst ();
		UnitMgr.InitInst ();
		EffectMgr.InitInst ();
		ElementMgr.InitInst ();
		ChipMgr.InitInst ();
		ObjectPool.InitInst ();
		CamMgr.InitInst ();

		m_audioBgm = transform.GetComponent<AudioSource> ();
		m_clipDeleteEnemy = Resources.Load<AudioClip> ("Sound/Bgm/EnemyDeleted");
		m_clipGameover = Resources.Load<AudioClip> ("Sound/Bgm/Gameover");
	}
	// Use this for initialization
	void Start () {

		DBMgr.GetInst ().OpenConnection ();

		m_audioBgm.enabled = false;

		MapMgr.GetInst ().Initialize ();
		MapMgr.GetInst ().CreateMap ();

		CamMgr.GetInst ().Initialize ();
		ObjectPool.GetInst ().Initialzie ();
		EffectMgr.GetInst ().Initailzie ();
		ElementMgr.GetInst ().Initialize ();
		ChipMgr.GetInst ().Initialize ();
		UnitMgr.GetInst ().Initialize ();

		EffectMgr.GetInst ().SetEffectPooled ();
		ObjectPool.GetInst ().CreatePool ();

		StartCoroutine (UnitMgr.GetInst().GenUnit());
		UnitMgr.GetInst ().EnemyPlay ();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if(Application.platform == RuntimePlatform.Android)
		{
			if(Input.GetKey(KeyCode.Escape))
			{
				UIMgr.GetInst ().SetActivePause ();
				if(UIMgr.GetInst().GetCustomed()==false)
				{
					TimeScaleChange ();
				}
			}
		}
	}

	public IEnumerator ClearStage()
	{
		yield return null;
		m_bplay = false;
		UIMgr.GetInst ().SetActiveMain (false);
		yield return null;
		StageMgr.GetInst ().ChangeEnemyDeleteBgm ();
		UnitMgr.GetInst().GetPlayer().GetAnim ().speed = 0.0f;
		UIMgr.GetInst ().GetMsgUI ().SetActive (true);
		UIMgr.GetInst ().GetAnimMsgUI ().SetInteger ("CurAnim", 3);
		yield return new WaitForSeconds(3.0f);
		UIMgr.GetInst ().GetAnimStateUI ().SetInteger ("CurAnim",1);
		yield return new WaitForSeconds (5.0f);
		ReleaseStage ();
		yield return null;
	}
	public IEnumerator GameOver()
	{
		UIMgr.GetInst ().GetMsgUI ().SetActive (true);
		UIMgr.GetInst ().GetAnimMsgUI ().SetInteger ("CurAnim", 4);
		m_bplay = false;
		yield return new WaitForSeconds (2.0f);
		m_audioBgm.Stop ();
		yield return new WaitForSeconds (3.0f);
		UIMgr.GetInst ().GetAnimStateUI ().SetInteger ("CurAnim",2);
		yield return new WaitForSeconds (1.0f);
		ChangeBgmGameover ();
		yield return new WaitForSeconds (3.0f);
		ReleaseStage ();
		yield return null;

	}

	public void ReleaseStage()
	{
		UIMgr.GetInst ().ClearChip ();
		UIMgr.GetInst ().ClearSelectedIcons ();
		UIMgr.GetInst ().ClearUseChips ();
		UnitMgr.GetInst ().ReleaseUnit ();
		EffectMgr.GetInst ().ReleaseEffect ();
		ElementMgr.GetInst ().ReleaseElement ();
		ChipMgr.GetInst ().ReleaseChipList ();
		DBMgr.GetInst ().CloseConnection ();	
		ObjectPool.GetInst ().ReleasePooled ();
		System.GC.Collect ();
		UnityEngine.SceneManagement.SceneManager.LoadScene("MainScene");
	}

	public void TimeScaleChange()
	{
		if(Time.timeScale==0.0f)
		{
			Time.timeScale = 1.0f;
		}
		else
		{
			Time.timeScale = 0.0f;
		}
	}
		
	public bool GetPlay()
	{
		return m_bplay;
	}
		
	public IEnumerator SetStart()
	{
		yield return null;
		UIMgr.GetInst ().GetMsgUI ().SetActive (true);
		UIMgr.GetInst ().GetAnimMsgUI ().SetInteger ("CurAnim", 1);
		yield return new WaitForSeconds (1.0f);
		UIMgr.GetInst ().GetAudioMsgUI ().enabled = true;
		yield return new WaitForSeconds(1.0f);
		UIMgr.GetInst ().GetAudioMsgUI ().enabled = false;
		UIMgr.GetInst ().GetMsgUI ().SetActive (false);
		UnitMgr.GetInst().GetPlayer().GetAnim ().speed = 1.0f;
		for(int i=0;i<UnitMgr.GetInst().GetEnemyList().Count;i++)
		{
			UnitMgr.GetInst().GetEnemyList()[i].GetAnim ().speed = 1.0f;
		}
		Time.timeScale = 0.0f;
		UIMgr.GetInst ().SetActiveCustom ();

		StageMgr.GetInst ().SetStart ();
		yield return null;
		m_audioBgm.enabled=true;
		m_bplay = true;
		StartCoroutine(UnitMgr.GetInst().EnemyPlay());
		yield return null;
	}

	public int GetCurTurn()
	{
		return m_nCurTrun;
	}

	public void PlusCurTurn()
	{
		m_nCurTrun++;
	}

	private void ChangeEnemyDeleteBgm()
	{
		m_audioBgm.clip = m_clipDeleteEnemy;
		m_audioBgm.Play ();
	}

	private void ChangeBgmGameover()
	{
		m_audioBgm.PlayOneShot (m_clipGameover);
	}



}
