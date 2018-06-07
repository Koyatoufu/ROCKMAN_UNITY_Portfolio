using UnityEngine;
using System.Xml;
using System.Collections;
using System.Collections.Generic;

public class ChipXMlFileMgr {

	private ChipXMlFileMgr()
	{
		
	}

	public static void LoadXml(string szPath,List<ChipData> ChipInfos)
	{
		XmlDocument xmlDocument = new XmlDocument ();
		xmlDocument.Load (szPath);

		XmlNode rootNode = xmlDocument.SelectSingleNode ("ChipList");

		XmlNodeList xmlChipInfo = rootNode.SelectNodes ("ChipInfo");

		for(int i=0;i<xmlChipInfo.Count;i++)
		{

			ChipData chipTmp=new ChipData();

			chipTmp.eChipLabel=(E_CHIPLABEL)int.Parse(xmlChipInfo[i].ChildNodes [0].InnerText);
			chipTmp.nID = int.Parse (xmlChipInfo[i].ChildNodes[1].InnerText);
			chipTmp.nImgResourceID = int.Parse (xmlChipInfo[i].ChildNodes[2].InnerText);
			chipTmp.nIconResourceID = int.Parse (xmlChipInfo[i].ChildNodes[3].InnerText);
			chipTmp.nCodeIndex = int.Parse (xmlChipInfo[i].ChildNodes[4].InnerText);
			chipTmp.nValue = -1;

			ChipInfos.Insert (i,chipTmp);

		}
	}

	public static void LoadXml(TextAsset textAsset,List<ChipData> ChipInfos)
	{
		XmlDocument xmlDocument = new XmlDocument ();
		xmlDocument.LoadXml (textAsset.text);

		XmlNode rootNode = xmlDocument.SelectSingleNode ("ChipList");

		XmlNodeList xmlChipInfo = rootNode.SelectNodes ("ChipInfo");

		for(int i=0;i<xmlChipInfo.Count;i++)
		{

			ChipData chipTmp=new ChipData();

			chipTmp.eChipLabel=(E_CHIPLABEL)int.Parse(xmlChipInfo[i].ChildNodes [0].InnerText);
			chipTmp.nID = int.Parse (xmlChipInfo[i].ChildNodes[1].InnerText);
			chipTmp.nImgResourceID = int.Parse (xmlChipInfo[i].ChildNodes[2].InnerText);
			chipTmp.nIconResourceID = int.Parse (xmlChipInfo[i].ChildNodes[3].InnerText);
			chipTmp.nCodeIndex = int.Parse (xmlChipInfo[i].ChildNodes[4].InnerText);
			chipTmp.nValue = -1;

			ChipInfos.Insert (i,chipTmp);

		}
	}


	public static void WriteXml(string szPath,List<ChipData> ChipInfos)
	{
		XmlDocument xmlDocument = new XmlDocument ();
		xmlDocument.AppendChild (xmlDocument.CreateXmlDeclaration("1.0","utf-8","yes"));

		XmlNode rootNode = xmlDocument.CreateNode (XmlNodeType.Element,"ChipList",string.Empty);
		xmlDocument.AppendChild (rootNode);


		for(int i=0;i<ChipInfos.Count;i++)
		{
			XmlNode xmlChipInfo= xmlDocument.CreateNode(XmlNodeType.Element,"ChipInfo",string.Empty);
			rootNode.AppendChild (xmlChipInfo);

			XmlElement xmlEleLabel = xmlDocument.CreateElement ("Label");
			xmlEleLabel.InnerText = ((int)(ChipInfos [i].eChipLabel)).ToString();
			xmlChipInfo.AppendChild (xmlEleLabel);

			XmlElement xmlEleID = xmlDocument.CreateElement ("ID");
			xmlEleID.InnerText = ChipInfos [i].nID.ToString ();
			xmlChipInfo.AppendChild (xmlEleID);

			XmlElement xmlEleImgResID = xmlDocument.CreateElement ("ImgResourceID");
			xmlEleImgResID.InnerText = ChipInfos [i].nImgResourceID.ToString ();
			xmlChipInfo.AppendChild (xmlEleImgResID);

			XmlElement xmlEleIconResID = xmlDocument.CreateElement ("IconResourceID");
			xmlEleIconResID.InnerText = ChipInfos [i].nIconResourceID.ToString ();
			xmlChipInfo.AppendChild (xmlEleIconResID);

			XmlElement xmlEleCodeIdx = xmlDocument.CreateElement ("CodeIndex");
			xmlEleCodeIdx.InnerText = ChipInfos [i].nCodeIndex.ToString ();
			xmlChipInfo.AppendChild (xmlEleCodeIdx);

		}

		xmlDocument.Save (szPath);
	}
}

/*
<ChipLIst>
<ChipInfo>
	<Label>
	<ID>
	<ImgResourceID>
	<IconResourceID>
	<CodeIndex>
*/