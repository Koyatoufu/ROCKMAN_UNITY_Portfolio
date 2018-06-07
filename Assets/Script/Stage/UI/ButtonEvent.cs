using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ButtonEvent : MonoBehaviour {

	AudioClip m_audioSelect;
	AudioClip m_audioConfirm;
	AudioClip m_audioPause;
	AudioClip m_audioRemoveChip;
	AudioClip m_audioResume;

	AudioSource m_audioButton;

	void Awake()
	{
		m_audioSelect = Resources.Load <AudioClip>("Sound/Wav/Select");
		m_audioConfirm = Resources.Load <AudioClip> ("Sound/Wav/Confirm");
		m_audioPause = Resources.Load <AudioClip> ("Sound/Wav/Pause");
		m_audioRemoveChip = Resources.Load <AudioClip> ("Sound/Wav/Cancel");
		m_audioResume = Resources.Load <AudioClip> ("Sound/Wav/Resume");

		m_audioButton = transform.gameObject.GetComponent<AudioSource> ();
	}

	public void PauseGame()
	{
		if(StageMgr.GetInst().GetPlay()==false)
		{
			return;
		}
		m_audioButton.PlayOneShot(m_audioPause);

		UIMgr.GetInst ().SetActivePause ();
		if(UIMgr.GetInst().GetCustomed()==false)
		{
			StageMgr.GetInst ().TimeScaleChange ();
		}
	}

	public void UseChip(int nIndex)
	{
		if(StageMgr.GetInst().GetPlay()==false)
		{
			return;
		}

		StartCoroutine (((PlayerUnit)UnitMgr.GetInst ().GetPlayer ()).UseChip (nIndex));
	}
	public void SelectChip(int nIndex)
	{
		ChipMgr.GetInst ().SelectChip (nIndex);
		m_audioButton.clip = m_audioSelect;
		m_audioButton.Play ();
	}
	public void RemoveChip(int nIndex)
	{
		ChipMgr.GetInst ().RemoveUseCHips (nIndex);
		m_audioButton.clip = m_audioRemoveChip;
		m_audioButton.Play ();
	}

	public void LockEnemy()
	{
		if(ChipMgr.GetInst().IsEmptyUseChips())
			return;
		
		Debug.Log ("Lock");
		((PlayerUnit)UnitMgr.GetInst ().GetPlayer ()).SetLock ();
	}
	public void AtackPlayer()
	{
		if(StageMgr.GetInst().GetPlay()==false)
		{
			return;
		}
		UnitMgr.GetInst ().GetPlayer ().SetAct (E_ACT.ATK);
		UnitMgr.GetInst ().GetPlayer ().StartCoroutine ("AttackUnit");
	}
	public void StopAttackPlayer()
	{
		UnitMgr.GetInst ().GetPlayer ().SetAct (E_ACT.IDLE);
		UnitMgr.GetInst ().GetPlayer ().StopCoroutine ("AttackUnit");
		UnitMgr.GetInst ().GetPlayer ().GetAnim().SetInteger ("CurAnim", (int)E_PlAnimState.IDLE);
	}

	public void UseShield()
	{
		UnitMgr.GetInst ().GetPlayer ().StartCoroutine ("UseShield");
	}

	public void CustomConfirm()
	{
		UIMgr.GetInst ().SetActiveCustom ();
		UIMgr.GetInst ().SetActiveMain ();
		UIMgr.GetInst ().SetCustomConfirm ();

		StageMgr.GetInst ().TimeScaleChange ();

		UIMgr.GetInst ().GetGaugeImage ().fillAmount = 0.0f;

		ChipMgr.GetInst ().ConfirmUseChips ();

		m_audioButton.clip = m_audioConfirm;
		m_audioButton.Play ();
	}

	public void GaugeBtn()
	{
		if(StageMgr.GetInst().GetPlay()==false)
		{
			return;
		}
		if(UIMgr.GetInst().GetGaugeAnim().GetBool("Full"))
		{
			UIMgr.GetInst ().SetActiveCustom ();
			UIMgr.GetInst ().SetActiveMain ();

			StageMgr.GetInst ().TimeScaleChange ();

			ChipMgr.GetInst ().ClearUseChipData ();
			ChipMgr.GetInst ().StartCustomChipSet ();
		}
		m_audioButton.clip = m_audioPause;
		m_audioButton.Play ();
	}

	public void ReturnTitle()
	{
		StageMgr.GetInst ().TimeScaleChange ();
		m_audioButton.clip = m_audioResume;
		m_audioButton.Play ();
		StageMgr.GetInst ().ReleaseStage ();
	}
}
