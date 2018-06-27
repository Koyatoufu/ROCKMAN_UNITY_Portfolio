using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ObjectPool{

	private struct S_OBJECT
	{
		public GameObject prefabs;
		public int nBufferAmount;
	}

	private static ObjectPool m_Inst = null;

	private GameObject m_ContainObject;

	private S_OBJECT[] m_objects;
	private List<GameObject>[] m_pooledList;
	private const int m_nDefaultBufferamount=15;

	private bool m_bCanGrow;

	public static void InitInst()
	{
		if(m_Inst==null)
		{
			m_Inst = new ObjectPool ();
		}
	}

	public static ObjectPool GetInst()
	{
		return m_Inst;
	}

	private ObjectPool()
	{
		m_ContainObject = null;
		m_objects = null;
		m_pooledList = null;
		m_bCanGrow = false;
	}

	public void Initialzie()
	{
		m_ContainObject = new GameObject ("ObjectPool");

		m_objects = new S_OBJECT[50];
		for(int i=0;i<m_objects.Length;i++)
		{
			m_objects [i].prefabs = null;
			m_objects [i].nBufferAmount = 0;
		}

		m_pooledList = new List<GameObject>[m_objects.Length];

		m_bCanGrow = true;
	}

	public void SetPrefabs(GameObject obj,int nAmountSize=0)
	{
		if(obj==null)
		{
			Debug.Log (obj);
			return;
		}

		for (int i = 0; i < m_objects.Length; i++) 
		{
			if(m_objects[i].prefabs!=null)
			{
				if(m_objects[i].prefabs.name.CompareTo(obj.name)==0)
				{
					return;
				}
			}
		}
		for(int i=0;i<m_objects.Length;i++)
		{
			if (m_objects [i].prefabs == null) {
				m_objects [i].prefabs= obj;
				m_objects [i].nBufferAmount = nAmountSize;
				if(nAmountSize==0)
				{
					m_objects [i].nBufferAmount = m_nDefaultBufferamount;
				}
				return;
			}
		}
	}

	public void ResizeFrefebs()
	{
		S_OBJECT[] tmp=new S_OBJECT[m_objects.Length+1];
		for (int i = 0; i < m_objects.Length; i++) 
		{
			tmp [i] = m_objects [i];
		}
		m_objects = tmp;
		//System.GC.Collect ();
	}

	public void CreatePool()
	{
		for(int i=0;i<m_objects.Length;i++)
		{
			m_pooledList[i]=new List<GameObject>();

			int nBufferAmount = m_nDefaultBufferamount;

			if(m_objects[i].nBufferAmount>0)
			{
				nBufferAmount = m_objects[i].nBufferAmount;
			}

			if(m_objects[i].prefabs!=null)
			{
				for(int j=0;j<nBufferAmount;j++)
				{
                    GameObject obj = (GameObject.Instantiate (m_objects[i].prefabs));
					obj.name = m_objects[i].prefabs.name;
					PooledObject (obj);
				}
			}

		}
	}

	public void PooledObject(GameObject obj)
	{
		if(obj==null)
		{
			return;
		}

		for(int i=0;i<m_objects.Length;i++)
		{
            if (m_objects[i].prefabs == null)
                continue;

			if(m_objects[i].prefabs.name.CompareTo(obj.name)==0)
			{
				obj.SetActive(false);
				obj.transform.parent = m_ContainObject.transform;

				m_pooledList [i].Add (obj);
				return;
			}
		}

        GameObject.Destroy(obj);
	}
	public GameObject GetObject(GameObject obj)
	{
		for(int i=0;i<m_objects.Length;i++)
		{
			if(m_objects[i].prefabs.name.CompareTo(obj.name)==0)
			{
				if(m_pooledList[i].Count>0)
				{
					GameObject pooledObj = m_pooledList [i] [0];
					m_pooledList [i].RemoveAt (0);
					pooledObj.transform.parent = null;
					pooledObj.SetActive (true);
					return pooledObj;
				}
				else if(m_bCanGrow)
				{
					GameObject canObj = (GameObject)(MonoBehaviour.Instantiate (m_objects[i].prefabs));
					canObj.name = m_objects[i].prefabs.name;
					canObj.SetActive (true);
					return canObj;
				}
				break;
			}
		}

		return null;
	}

	public void ReleasePooled()
	{
		for(int i=0;i<m_pooledList.Length;i++)
		{
			m_pooledList [i].Clear ();
		}
	}
}
