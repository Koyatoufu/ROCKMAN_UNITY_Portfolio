using UnityEngine;
using System.Collections;

public class CamMgr {

	private GameObject m_goMainCam;
	private GameObject m_goSubCam;

	private Camera m_mainCam;
	private Camera m_subCam;

	private Skybox m_mainSkybox;
	private Skybox m_subSkybox;

	private bool m_bRotate;

	private float m_fMatX;
	private float m_fMatY;

	private static CamMgr mInst=null;

	private CamMgr()
	{
		m_goMainCam = null;
		m_goSubCam = null;
		m_mainCam = null;
		m_subCam = null;
		m_bRotate = false;
		m_mainSkybox = null;
		m_subSkybox = null;
		m_fMatX = 0.0f;
		m_fMatY = 0.0f;
	}

	public static void InitInst()
	{
		if(mInst==null)
		{
			mInst = new CamMgr ();
		}
	}
	public static CamMgr GetInst()
	{
		return mInst;
	}

	public void Initialize()
	{
		m_goMainCam = StageMgr.GetInst().transform.Find("Main Camera").gameObject;
		m_goSubCam = StageMgr.GetInst ().transform.Find("SubCamera").gameObject;
		m_bRotate=true;

		m_mainCam = m_goMainCam.GetComponent<Camera> ();
		m_subCam = m_goSubCam.GetComponent<Camera> ();

		m_goSubCam.SetActive (false);

		m_mainSkybox = m_mainCam.transform.GetComponent<Skybox>();
		m_subSkybox = m_subCam.transform.GetComponent<Skybox> ();
	}

	public IEnumerator RotateSky()
	{
		while(m_bRotate!=false)
		{
			m_fMatX += 0.25f;
			m_fMatY += 0.25f;
			m_mainSkybox.material.SetTextureOffset("_MainTex",new Vector2(m_fMatX,m_fMatY));
			m_subSkybox.material.SetTextureOffset("_MainTex",new Vector2(m_fMatX,m_fMatY));
			yield return null;
		}
		yield return null;
	}

	public GameObject GetMainCam()
	{
		return m_goMainCam;
	}
	public GameObject GetSubCam()
	{
		return m_goSubCam;
	}

	public Camera GetMainCameraComponent()
	{
		return m_mainCam;
	}

	public Camera GetSubCameraComponent()
	{
		return m_subCam;
	}

	public IEnumerator SubCamActive(Transform transform)
	{
		m_goSubCam.SetActive (true);
		m_goSubCam.transform.position=transform.position+new Vector3(-4.0f,2.5f,0.5f);
		m_goSubCam.transform.LookAt (transform.position);
		yield return new WaitForSeconds(1.0f);
		m_goSubCam.SetActive (false);
		yield return null;
	}
}
