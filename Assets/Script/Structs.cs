using System;
using UnityEngine;

public struct UnitStatus
{
	public string szName;
	public int nCurHp;
	public int nMaxHp;
	public float fMoveSpeed;
	public UnitStatus(string szName,int nCurHp,int nMaxHp,float fMoveSpeed)
	{
		this.szName=szName;
		this.nCurHp=nCurHp;
		this.nMaxHp=nMaxHp;
		this.fMoveSpeed=fMoveSpeed;
	}
}
public struct UnitAtk
{
	public int nID;
	public int nDmg;
	public float fSpeed;
	public E_TYPE eType;
	public GameObject objType;
	public UnitAtk(int nID,int nDmg,float fSpeed,E_TYPE eType,GameObject objType)
	{
		this.nID=nID;
		this.nDmg=nDmg;
		this.fSpeed = fSpeed;
		this.eType=eType;
		this.objType=objType;
	}
}

public struct ChipData
{
	public E_CHIPLABEL eChipLabel;

	public int nID;
	public string szName;

	public int nValue;
	public E_TYPE eType;
	public E_TYPE eType2;

	public int nMemory;
	public E_CHIPTYPE eChipType;
	public string szFileName;
	public GameObject objType;

	public int nImgResourceID;
	public int nImgID;

	public int nIconResourceID;
	public int nIconID;

	public int nCodeIndex;
	public char cCode;

	public int nAnimIndex;
	public bool bSelected;
	public ChipData(int nID,string szName,int nValue,E_TYPE eType,E_TYPE eType2,int nMemory,
		E_CHIPLABEL eChipLabel,E_CHIPTYPE eChipType,string szFileName,GameObject objType,
		int nImgResourceID,int nImgID,
		int nIconResourceID,int nIconID,
		int nCodeIndex,char cCode,int nAnimIndex,bool bSelected)
	{
		this.nID = nID;
		this.szName = szName;

		this.nValue = nValue;
		this.eType = eType;
		this.eType2 = eType2;
		this.nMemory=nMemory;

		this.eChipLabel = eChipLabel;
		this.eChipType = eChipType;
		this.szFileName = szFileName;
		this.objType = objType;

		this.nImgResourceID = nImgResourceID;
		this.nImgID = nImgID;

		this.nIconResourceID = nIconResourceID;
		this.nIconID = nIconID;

		this.nCodeIndex = nCodeIndex;
		this.cCode = cCode;
		this.nAnimIndex=nAnimIndex;
		this.bSelected = false;
	}
}