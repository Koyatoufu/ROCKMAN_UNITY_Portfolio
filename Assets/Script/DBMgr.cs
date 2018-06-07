using UnityEngine;
using Mono.Data.Sqlite;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System;
public class DBMgr {
	
	private static DBMgr m_Inst = null;

	private string m_szDBname;
	private SqliteConnection m_connection;
	private SqliteCommand m_command;
	private SqliteDataReader m_dataReader;

	private DBMgr()
	{
		m_szDBname = null;
		m_connection = null;
		m_command = null;
		m_dataReader = null;
	}

	public static void InitInst()
	{
		if(m_Inst==null)
		{
			m_Inst = new DBMgr ();
		}
	}

	public static DBMgr GetInst()
	{
		return m_Inst;
	}

	public void OpenConnection()
	{
		m_szDBname=Application.dataPath+"/StreamingAssets/Database.db";

		if(Application.platform==RuntimePlatform.Android)
		{
			m_szDBname = Application.persistentDataPath + "/Database.db";
			if(!System.IO.File.Exists(m_szDBname))
			{
				WWW loadDB = new WWW (Application.streamingAssetsPath+"/Database.db");
				Debug.Log ("jar:file//"+Application.dataPath+"!/assets/Database.db");
				Debug.Log (Application.streamingAssetsPath+"/Database.db");
				while(!loadDB.isDone){}
				Debug.Log (loadDB.text);
				Debug.Log (loadDB.bytes.Length+" Bytes");
				System.IO.File.WriteAllBytes(m_szDBname, loadDB.bytes);	
			}
		}

		m_szDBname = "URI=file:" + m_szDBname;

		m_connection = new SqliteConnection (m_szDBname);

		m_connection.Open();
	}

	public void CloseConnection()
	{
		if(m_dataReader!=null)
		{
			m_dataReader.Close ();
			m_dataReader = null;
		}
		if(m_command!=null)
		{
			m_command.Dispose ();
			m_command = null;
		}
		if(m_connection!=null)
		{
			m_connection.Close();
			m_connection=null;
		}
		System.GC.Collect();
	}

	public void CheckTable()
	{
		m_command = m_connection.CreateCommand ();
		m_command.CommandText = "SELECT name FROM sqlite_master";

		m_dataReader = m_command.ExecuteReader ();
		Debug.Log (m_dataReader.Read());
		while(m_dataReader.Read())
		{
			Debug.Log (m_dataReader.GetValue(0));
		}
	}

	public void SelectStandard(List<ChipData> listChips,int nIndex)
	{
		m_command = m_connection.CreateCommand ();
		string str = "Select * from StandardChip where ID="+listChips[nIndex].nID;
		m_command.CommandText = str;


		int [] arCodeDBIndex=new int[4];
		arCodeDBIndex [0] = 10;
		arCodeDBIndex [1] = 11;
		arCodeDBIndex [2] = 12;
		arCodeDBIndex [3] = 13;
	
		m_dataReader = m_command.ExecuteReader ();
		if(m_dataReader.Read ())
		{
			ChipData chipData = listChips[nIndex];

			chipData.szName=m_dataReader.GetString (1);
			chipData.eType = (E_TYPE)(m_dataReader.GetInt32 (2));
			if(!m_dataReader.GetValue(3).Equals(DBNull.Value))
				chipData.eType2 = (E_TYPE)(m_dataReader.GetInt16 (3));
			chipData.eChipType = (E_CHIPTYPE)(m_dataReader.GetInt32 (4));
			if(m_dataReader.GetValue(5).Equals(DBNull.Value)==false)
				chipData.nValue = m_dataReader.GetInt32 (5);
			chipData.nMemory = m_dataReader.GetInt32 (6);
			chipData.szFileName = m_dataReader.GetString (7);
			chipData.nImgID = m_dataReader.GetInt32 (8);
			chipData.nIconID = m_dataReader.GetInt32 (9);
			chipData.nAnimIndex = m_dataReader.GetInt32 (14);



			char cCode='*';
			if (m_dataReader.GetValue(arCodeDBIndex[chipData.nCodeIndex]).Equals(DBNull.Value))
				chipData.nCodeIndex = 0;
			else if (chipData.nCodeIndex > 3)
				chipData.nCodeIndex = 0;


			if(!m_dataReader.GetValue(arCodeDBIndex[chipData.nCodeIndex]).Equals(DBNull.Value))
				cCode=m_dataReader.GetString (arCodeDBIndex[chipData.nCodeIndex]).ToCharArray()[0];

			chipData.cCode = cCode;
		
			listChips [nIndex] = chipData;
		}
	}
	public void SelectStandard(int nID,int nCodeIndex)
	{
		m_command = m_connection.CreateCommand ();
		string str = "Select * from StandardChip where ID="+nID;
		m_command.CommandText = str;

		int [] arCodeDBIndex=new int[4];
		arCodeDBIndex [0] = 10;
		arCodeDBIndex [1] = 11;
		arCodeDBIndex [2] = 12;
		arCodeDBIndex [3] = 13;

		m_dataReader = m_command.ExecuteReader ();
		if(m_dataReader.Read ())
		{
			string szName=m_dataReader.GetString (1);
			char cCode=' ';
			if (m_dataReader.GetValue(arCodeDBIndex[nCodeIndex]).Equals(DBNull.Value))
				nCodeIndex = 0;
			else if (nCodeIndex > 3)
				nCodeIndex = 0;
			cCode=m_dataReader.GetString (arCodeDBIndex[nCodeIndex]).ToCharArray()[0];
			Debug.Log (szName+ " " +cCode);
		}
	}

	public void SelectEnemy(int nID)
	{
		m_command = m_connection.CreateCommand ();
		string str = "Select * from EnemyList where ID="+nID;
		m_command.CommandText = str;

		m_dataReader = m_command.ExecuteReader ();

		m_dataReader.Read ();
		Debug.Log (m_dataReader.GetString(1));
	}
}
