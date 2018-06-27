using UnityEngine;
using System.Collections;

public class StageMgr : Photon.MonoBehaviour {

	protected static StageMgr m_inst=null;
    public static StageMgr Inst { get { return m_inst; } }

	protected bool m_bplay = false;
    public bool IsPlay { get { return m_bplay; } }

	protected AudioSource m_audioBgm = null;
	protected AudioClip m_clipDeleteEnemy = null;
	protected AudioClip m_clipGameover;

	protected int m_nCurTrun=0;
    public int CurTurn { get { return m_nCurTrun; } }

	protected void Awake()
	{
        if(m_inst!=null)
        {
            Destroy(gameObject);
            return;
        }

        m_inst = this;

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
	protected void Start ()
    {

		DBMgr.GetInst ().OpenConnection ();

		m_audioBgm.enabled = false;

		MapMgr.Inst.Initialize ();
		MapMgr.Inst.CreateMap ();

		CamMgr.GetInst ().Initialize ();
		ObjectPool.GetInst ().Initialzie ();
		EffectMgr.GetInst ().Initailzie ();
		ElementMgr.GetInst ().Initialize ();
		ChipMgr.Inst.Initialize ();
		UnitMgr.Inst.Initialize ();

		EffectMgr.GetInst ().SetEffectPooled ();
		ObjectPool.GetInst ().CreatePool ();

		if(MultyManager.Inst==null)
        {
            StartCoroutine(UnitMgr.Inst.GenUnit());
           UnitMgr.Inst.EnemyPlay();
        }   
	}
	
    protected virtual void Update()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                UIMgr.Inst.SetActivePause();
                if (UIMgr.Inst.GetCustomed() == false)
                {
                    TimeScaleChange(Time.timeScale!=0f);
                }
            }
        }
    }

    public void ClearStage()
    {
        StartCoroutine(ClearStageCoroutine(3));
    }

	protected virtual IEnumerator ClearStageCoroutine(int nStateAnim)
	{
		yield return null;
		m_bplay = false;
		UIMgr.Inst.SetActiveMain (false);
		yield return null;
		Inst.ChangeEnemyDeleteBgm ();
		UnitMgr.Inst.Player.GetAnim ().speed = 0.0f;
		UIMgr.Inst.GetMsgUI ().SetActive (true);
		UIMgr.Inst.GetAnimMsgUI ().SetInteger ("CurAnim", 3);
		yield return new WaitForSeconds(3.0f);
		UIMgr.Inst.GetAnimStateUI ().SetInteger ("CurAnim",nStateAnim);
		yield return new WaitForSeconds (5.0f);
		ReleaseStage ();
		yield return null;
	}

    public void GameOver()
    {
        StartCoroutine(GameOverCoroutine(2));
    }

	protected virtual IEnumerator GameOverCoroutine(int nStateAnim)
	{
        yield return null;
		UIMgr.Inst.GetMsgUI ().SetActive (true);
		UIMgr.Inst.GetAnimMsgUI ().SetInteger ("CurAnim", 4);
		m_bplay = false;
        yield return null;
        m_audioBgm.Stop ();
		yield return new WaitForSeconds (3.0f);
		UIMgr.Inst.GetAnimStateUI ().SetInteger ("CurAnim",nStateAnim);
        yield return new WaitForSeconds(1.0f);
        ChangeBgmGameover ();
		yield return new WaitForSeconds (3.0f);
		ReleaseStage ();
		yield return null;
	}

	public virtual void ReleaseStage()
	{
		UIMgr.Inst.ClearChip ();
		UIMgr.Inst.ClearSelectedIcons ();
		UIMgr.Inst.ClearUseChips ();
		UnitMgr.Inst.ReleaseUnit ();
		EffectMgr.GetInst ().ReleaseEffect ();
		ElementMgr.GetInst ().ReleaseElement ();
		ChipMgr.Inst.ReleaseChipList ();
		DBMgr.GetInst ().CloseConnection ();	
		ObjectPool.GetInst ().ReleasePooled ();
		System.GC.Collect ();
        
        if(MultyManager.Inst==null)
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainScene");
    }

    public virtual void TimeScaleChange(bool isPause)
	{
        Time.timeScale = isPause ? 0f : 1f;
	}
		
    public void StageStart()
    {
        StartCoroutine(SetStartCoroutine());
    }

	protected IEnumerator SetStartCoroutine()
	{
		yield return null;
		UIMgr.Inst.GetMsgUI ().SetActive (true);
		UIMgr.Inst.GetAnimMsgUI ().SetInteger ("CurAnim", 1);
		yield return new WaitForSeconds (1.0f);
		UIMgr.Inst.GetAudioMsgUI ().enabled = true;
		yield return new WaitForSeconds(1.0f);
		UIMgr.Inst.GetAudioMsgUI ().enabled = false;
		UIMgr.Inst.GetMsgUI ().SetActive (false);
		Time.timeScale = 0.0f;
		UIMgr.Inst.SetActiveCustom ();

		yield return null;
		m_audioBgm.enabled=true;
		m_bplay = true;
        if(MultyManager.Inst==null)
        {
           UnitMgr.Inst.Player.GetAnim().speed = 1.0f;
            StartCoroutine(UnitMgr.Inst.EnemyPlay());
            for (int i = 0; i <UnitMgr.Inst.GetEnemyList().Count; i++)
            {
               UnitMgr.Inst.GetEnemyList()[i].GetAnim().speed = 1.0f;
            }
        }
		    
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

	protected void ChangeEnemyDeleteBgm()
	{
		m_audioBgm.clip = m_clipDeleteEnemy;
		m_audioBgm.Play ();
	}

    public virtual void ConfirmChips()
    {
        Debug.Log(UIMgr.Inst);
        Debug.Log(ChipMgr.Inst);

        if (UIMgr.Inst == null || ChipMgr.Inst == null)
            return;

        Debug.Log("Confirm Test");

        UIMgr.Inst.SetActiveCustom();
        UIMgr.Inst.SetActiveMain();
        UIMgr.Inst.SetCustomConfirm();

        UIMgr.Inst.GetGaugeImage().fillAmount = 0.0f;

        ChipMgr.Inst.ConfirmUseChips();

        TimeScaleChange(false);
    }

	protected void ChangeBgmGameover()
	{
		m_audioBgm.PlayOneShot (m_clipGameover);
	}

}
