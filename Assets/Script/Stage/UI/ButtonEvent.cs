using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ButtonEvent : Photon.MonoBehaviour {

    [SerializeField]
	AudioClip m_audioSelect = null;
    [SerializeField]
	AudioClip m_audioRemoveChip = null;

	AudioSource m_audioButton = null;

	void Awake()
	{
		m_audioButton = transform.gameObject.GetComponent<AudioSource> ();
	}

	public virtual void PauseGame(bool isPause)
	{
        if (StageMgr.Inst.IsPlay == false)
        {
            return;
        }

        UIMgr.Inst.SetActivePause();
        if (UIMgr.Inst.GetCustomed() == false)
        {
            StageMgr.Inst.TimeScaleChange(isPause);
        }
    }

	public virtual void UseChip(int nIndex)
	{
		if(StageMgr.Inst.IsPlay==false)
		{
			return;
		}

		((PlayerUnit)UnitMgr.Inst.Player).UseChip (nIndex);
	}
	public void SelectChip(int nIndex)
	{
		ChipMgr.Inst.SelectChip (nIndex);
		m_audioButton.clip = m_audioSelect;
		m_audioButton.Play ();
	}
	public void RemoveChip(int nIndex)
	{
		ChipMgr.Inst.RemoveUseCHips (nIndex);
		m_audioButton.clip = m_audioRemoveChip;
		m_audioButton.Play ();
	}
    	
	public virtual void AtackPlayer()
	{
		if(StageMgr.Inst.IsPlay==false)
		{
			return;
		}

        PlayerUnit player =UnitMgr.Inst.Player as PlayerUnit;

        player.AttackUnit();

	}
	public virtual void StopAttackPlayer()
	{
        PlayerUnit player =UnitMgr.Inst.Player as PlayerUnit;

        if (player == null)
            return;

        player.StopAttack();
    }

	public virtual void UseShield()
	{
        PlayerUnit player = UnitMgr.Inst.Player as PlayerUnit;

        if (player == null)
            return;

        player.UseShield();
	}

	public void CustomConfirm()
	{
        Debug.Log("Parent");
        StageMgr.Inst.ConfirmChips();
    }

	public virtual void GaugeBtn()
	{
		if(StageMgr.Inst.IsPlay==false)
		{
			return;
		}
		if(UIMgr.Inst.GetGaugeAnim().GetBool("Full"))
		{
			UIMgr.Inst.SetActiveCustom ();
			UIMgr.Inst.SetActiveMain ();

			StageMgr.Inst.TimeScaleChange (true);

			ChipMgr.Inst.ClearUseChipData ();
			ChipMgr.Inst.StartCustomChipSet ();
		}
	}

	public virtual void ReturnTitle()
	{
		StageMgr.Inst.TimeScaleChange (false);

		StageMgr.Inst.ReleaseStage ();
    }

    public void ButtonSoundPlay(AudioClip clip)
    {
        if (clip == null)
            return;

        m_audioButton.clip = clip;
        m_audioButton.Play();
    }
}
