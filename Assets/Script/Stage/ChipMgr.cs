using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ChipMgr {

	private static ChipMgr m_Inst = null;

	private List<ChipData> m_listChips=null;
	private List<USECHIPS> m_useChips=null;

	private Sprite[][] m_arChipImg;
	private Sprite[] m_arIcon;

	private struct USECHIPS
	{
		public ChipData chipData;
		public int nKeys;
		public USECHIPS(ChipData chipData,int nKey)
		{
			this.chipData=chipData;
			this.nKeys=nKey;
		}
	}

	private ChipMgr()
	{
		m_listChips=new List<ChipData>();
		m_useChips = new List<USECHIPS> ();
	}

	public static void InitInst()
	{
		if(m_Inst==null)
		{
			m_Inst = new ChipMgr ();
		}
	}

	public static ChipMgr GetInst()
	{
		return m_Inst;
	}

	public void Initialize()
	{
		m_arChipImg= new Sprite[3][];
		for(int i=0;i<3;i++)
		{
			string szFilePath = "Texture/ChipImg/Standard_" + i;
			Sprite[] arTmp = Resources.LoadAll<Sprite> (szFilePath);
			m_arChipImg [i] = arTmp;
		}

		m_arIcon = Resources.LoadAll<Sprite> ("Texture/ChipImg/StandardIcon");

		m_useChips.Capacity = 5;

		TextAsset textTmp = Resources.Load <TextAsset>("XML/BaseFolder");
		
		ChipXMlFileMgr.LoadXml(textTmp,m_listChips);

		for(int i=0;i<m_listChips.Count;i++)
		{
			DBMgr.GetInst ().SelectStandard (m_listChips,i);
		}

		SuffleChip ();

		SetPrefebs ();

		StartCustomChipSet ();
	}

	public void StartCustomChipSet()
	{
		int nCount = 10;
		if(m_listChips.Count<10)
		{
			nCount = m_listChips.Count;
		}

		for(int i=0;i<nCount;i++)
		{
			UIMgr.GetInst ().GetImageIcon () [i].sprite = m_arIcon [m_listChips[i].nIconID];
			UIMgr.GetInst ().GetCodeText () [i].text = m_listChips [i].cCode.ToString ();
		}
	}

	public ChipData GetChipData(int nIndex)
	{
		return m_listChips [nIndex];
	}

	public List<ChipData> GetChipList()
	{
		return m_listChips;
	}
	public bool IsEmptyUseChips()
	{
		if(m_useChips.Count==0)
		{
			return true;
		}

		return false;
	}

	public void SelectChip(int nIndex)
	{
		if(nIndex>=m_listChips.Count)
			return;
		if(m_useChips.Count>4)
			return;
		if (m_listChips [nIndex].bSelected == true)
			return;
			
		UIMgr.GetInst ().SetImageChips (m_arChipImg[m_listChips[nIndex].nImgResourceID][m_listChips[nIndex].nImgID]);

		USECHIPS useChips = new USECHIPS (m_listChips[nIndex],nIndex);

		m_useChips.Insert (m_useChips.Count,useChips);

		UIMgr.GetInst ().GetSelectedIcons (m_useChips.Count-1).sprite = m_arIcon [(m_useChips[m_useChips.Count-1].chipData.nIconID)];
		int nValue = m_useChips [m_useChips.Count - 1].chipData.nValue;
		string szValue = " ";
		if(nValue!=-1&&nValue!=0)
		{
			szValue = nValue.ToString ();
		}
		UIMgr.GetInst().GetSelectChipText().text=m_useChips[m_useChips.Count-1].chipData.szName+" "+szValue;

		SetListChipbSelect (nIndex, true);
		UIMgr.GetInst ().GetImageIcon () [nIndex].color = Color.gray;

		if(m_listChips[nIndex].cCode!='*')
		{
			int nCount = 10;
			if(m_listChips.Count<nCount)
			{
				nCount = m_listChips.Count;
			}

			for(int i=0;i<nCount;i++)
			{
				if(m_listChips[i].bSelected==false)
				{
					if (m_listChips [i].cCode == m_listChips [nIndex].cCode)
						continue;
					else if(m_listChips[i].cCode=='*')
						continue;
					else 
					{
						if(m_listChips[i].nID!=m_listChips[nIndex].nID)
						{
							SetListChipbSelect (i, true);
							UIMgr.GetInst ().GetImageIcon () [i].color = Color.gray;
						}
					}
				}
			}
		}

		if(m_useChips.Count>4)
		{
			for(int i=0;i<5;i++)
			{
				UIMgr.GetInst ().GetImageIcon () [i].color = Color.gray;
			}
		}
	}

	public void RemoveUseCHips(int nIndex)
	{
		if(m_useChips.Count<=nIndex)
		{
			return;
		}
			
		SetListChipbSelect (m_useChips[nIndex].nKeys, false);
		UIMgr.GetInst ().GetImageIcon () [m_useChips[nIndex].nKeys].color = Color.white;
		m_useChips.RemoveAt (nIndex);
		UIMgr.GetInst ().ClearSelectedIcons ();	
		if (m_useChips.Count != 0) {
			for (int i = 0; i < m_useChips.Count; i++) {
				UIMgr.GetInst ().GetSelectedIcons (i).sprite = m_arIcon [(m_useChips [i].chipData.nIconID)];
			}

			UIMgr.GetInst ().SetImageChips (m_arChipImg [m_useChips [m_useChips.Count - 1].chipData.nImgResourceID] [m_useChips [m_useChips.Count - 1].chipData.nImgID]);
			int nValue = m_useChips [m_useChips.Count - 1].chipData.nValue;
			string szValue = " ";
			if(nValue!=-1&&nValue!=0)
			{
				szValue = nValue.ToString ();
			}
			UIMgr.GetInst().GetSelectChipText().text=m_useChips[m_useChips.Count-1].chipData.szName+" "+szValue;
		} 
		else {
			for(int i=0;i<10;i++)
			{
				SetListChipbSelect (i, false);
				UIMgr.GetInst ().GetImageIcon () [i].color = Color.white;
			}
		}

	}

	public void ConfirmUseChips()
	{

		UIMgr.GetInst ().GetCardInfoText ().text ="CardInfomation";

		if(m_useChips.Count==0)
			return;

		for(int i=0;i<m_useChips.Count;i++)
		{
			UIMgr.GetInst ().GetUseIcons (i).sprite = m_arIcon [(m_useChips [i].chipData.nIconID)];
		}

		int[] arData = new int[m_useChips.Count];

		for(int i=0;i<m_useChips.Count;i++)
		{
			arData [i] = m_useChips [i].nKeys;
		}

		QuickSort (arData, 0, arData.Length-1);

		for(int i=0;i<arData.Length;i++)
		{
			m_listChips.RemoveAt (arData [i]);
		}
			
		UIMgr.GetInst ().GetCardInfoText ().text = m_useChips [0].chipData.szName + " " + m_useChips [0].chipData.nValue;
	}

	public ChipData GetUseChipUse(int nIndex)
	{
		ChipData chipTmp=new ChipData();
		if(m_useChips.Count==0)
		{
			chipTmp.eChipType = E_CHIPTYPE.NONE;
			return chipTmp;
		}
		chipTmp = m_useChips [nIndex].chipData;
		m_useChips.RemoveAt (nIndex);
		return chipTmp;
	}

	public ChipData GetUseChip(int nIndex)
	{
		if(m_useChips.Count==0)
		{
			ChipData chipTmp = new ChipData ();
			chipTmp.eChipType = E_CHIPTYPE.NONE;
			return chipTmp;
		}
		if(m_useChips.Count<=nIndex)
		{
			ChipData chipTmp = new ChipData ();
			chipTmp.eChipType = E_CHIPTYPE.NONE;
			return chipTmp;
		}

		return m_useChips [nIndex].chipData;
	}
	public void SetUseChip(int nIndex,ChipData chipData)
	{
		USECHIPS useTmp = new USECHIPS (chipData,m_useChips[nIndex].nKeys);
		m_useChips [nIndex] = useTmp;
	}

	public void ClearUseChipData()
	{
		m_useChips.Clear ();
		UIMgr.GetInst ().ClearChip ();
		UIMgr.GetInst ().ClearSelectedIcons ();
		UIMgr.GetInst ().ClearUseChips ();

		int nCount = 10;

		if(m_listChips.Count<10)
		{
			nCount = m_listChips.Count;
		}
		for(int i=0;i<nCount;i++)
		{
			SetListChipbSelect (i, false);
			UIMgr.GetInst ().GetImageIcon () [i].color = Color.white;
		}
	}

	public void ResetUseChip()
	{
		UIMgr.GetInst ().ClearUseChips ();
		for(int i=0;i<m_useChips.Count;i++)
		{
			UIMgr.GetInst ().GetUseIcons (i).sprite = m_arIcon [(m_useChips [i].chipData.nIconID)];
		}

		UIMgr.GetInst ().GetCardInfoText ().text ="CardInfomation";

		if(m_useChips.Count!=0)
		{
			int nValue = m_useChips [0].chipData.nValue;
			string szValue = " ";
			if(nValue!=-1&&nValue!=0)
			{
				szValue=nValue.ToString();
			}
			UIMgr.GetInst ().GetCardInfoText ().text = m_useChips [0].chipData.szName + " " + szValue;
		}
	}

	public Sprite[][] GetChipImages()
	{
		return m_arChipImg;
	}
	public Sprite[] GetChipIcons()
	{
		return m_arIcon;
	}
	private void SuffleChip()
	{

		for(int i=m_listChips.Count-1;i>0;i--)
		{
			int nRandom=Random.Range (0, i);

			ChipData chipTmp=m_listChips[i];

			m_listChips[i]=m_listChips[nRandom];
			m_listChips [nRandom] = chipTmp;
		}

	}
	private void SetPrefebs()
	{
		for(int i=0;i<m_listChips.Count;i++)
		{
			string szTmp="Prefebs/Chip/"+m_listChips[i].szFileName;
			ChipData ChipTmp = m_listChips [i];
			ChipTmp.objType=Resources.Load<GameObject> (szTmp);
			m_listChips [i] = ChipTmp;
			ObjectPool.GetInst ().SetPrefabs (m_listChips[i].objType);
		}
	}
	private void SetListChipbSelect(int nIndex,bool bSelected)
	{
		ChipData chipTmp = m_listChips [nIndex];
		chipTmp.bSelected = bSelected;
		m_listChips [nIndex] = chipTmp;
	}
		

	private void QuickSort(int [] arData,int nLeft,int nRight)
	{
		
		if(nRight < nLeft)
		{
			return;
		}

		int nPivot = arData[nLeft];

		int nLow = nLeft;
		int nHigh = nRight;


		while (nLow < nHigh)
		{
			while (nLow <nHigh && arData[nHigh] < nPivot)
				--nHigh;
			arData[nLow] = arData[nHigh];

			while (nLow <nHigh && arData[nLow] > nPivot)
				++nLow;
			arData[nHigh] = arData[nLow];
		}
		arData[nLow] = nPivot;

		QuickSort(arData, nLeft, nLow - 1);
		QuickSort(arData, nLow + 1, nRight);

	}

	private void SwapInt(int [] arData,int nDst,int nSrc)
	{
		int nTmp = arData[nDst];
		arData [nDst] = arData [nSrc];
		arData [nSrc] = nTmp;
	}

	public void ReleaseChipList()
	{
		m_listChips.Clear ();
		m_useChips.Clear ();
	}
}
//Sprite[] arTmp = Resources.LoadAll<Sprite> ("Texture/UI/Stage/CustomBar");
//Debug.Log (arTmp[0]);


//UIMgr.GetInst ().SetImageChips (m_arChipImg[m_listChips[nIndex].nImgResourceID][m_listChips[nIndex].nImgID]);
//m_dicUseChips.Add (nIndex,m_listChips[nIndex]);
